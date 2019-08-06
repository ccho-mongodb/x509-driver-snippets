# x509-driver-snippets
Connection code snippets for all mongodb drivers using x.509 authentication

###Task description:

Driver connection snippets should be single-sourced for both Driver landing pages and connection snippets in the Atlas docs. Connection snippets should include how to connect to Atlas with both password authentication and x.509. The x.509 snippets will be shared with the Atlas team to update their UI.
Details:
*  Create new shared connection snippets file
*  All drivers have example to connect to Atlas with password
*  All drivers have example to connect to Atlas with x.509
A/C:
*  As a user, I can choose a connection snippet that shows how to connect to Atlas using password authentication or x.509 authentication for the driver of my choice.

External Reviewers: Jonathan DeStefano and Driver teams.


### Setup local hostname
Create hostnames in `/etc/hosts`

### Certificate creation:

https://docs.mongodb.com/manual/appendix/security/appendixA-openssl-ca/ (create a certificate authority certificate)


https://docs.mongodb.com/manual/appendix/security/appendixB-openssl-server/#appendix-server-certificate (create openssl server certificate)

https://docs.mongodb.com/manual/appendix/security/appendixC-openssl-client/#appendix-client-certificate (create openssl client certificate)

#### Start server
mongod -dbpath /Users/ccho/dev/drivers/test_db_data -logpath <path>/mongod.log --sslMode requireSSL --clusterAuthMode=x509 --sslPEMKeyFile <path>/test-server1.pem --sslCAFile <path>/test-ca.pem -fork

#### Create an admin user
https://docs.mongodb.com/manual/tutorial/configure-x509-client-authentication/

```
db.getSiblingDB("$external").runCommand(
  {
    createUser: "CN=ChrisChoClient,OU=TestClientCertificateOrgUnit,O=TestClientCertificateOrg,L=TestClientCertificateLocality,ST=TestClientCertificateState,C=US",
    roles: [
         { role: "readWrite", db: "test" },
         { role: "userAdminAnyDatabase", db: "admin" }
    ],
    writeConcern: { w: "majority" , wtimeout: 5000 }
  }
)
```


#### Test auth of admin user
```
mongo --ssl --sslPEMKeyFile <path>/test-server1.pem --sslCAFile <path>/test-ca.pem --host localmongo1  --authenticationMechanism MONGODB-X509 --authenticationDatabase='$external'
```


### On building keystore and truststore

#### Bundle the client and intermediate authority certs
```
$ cat mongodb-test-client.crt mongodb-test-ia.crt > client-bundle.crt
```

#### Convert to pfx

```
$ openssl pkcs12 -export -out client-certificate.pfx -inkey mongodb-test-client.key -in client-bundle.crt
```
`<choose a password>`


#### Convert pfx to pkcs12
```
$ keytool -importkeystore -destkeystore client.keystore -srckeystore client-certificate.pfx -srcstoretype pkcs12 -alias client-cert
```


#### confirm it: 
```
$ keytool -list -keystore client.keystore
```

#### Add server and client certs to truststore

```
$ keytool -import -alias server-cert -file test-server1.pem -keystore client.truststore
```

```
$ keytool -import -alias client-cert -file test-client.pem -keystore client.truststore
```
