# x509-driver-snippets
Connection code snippets for MongoDB drivers using x.509 authentication

# Setup

## Start mongod without TLS

## Add an admin user
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

## Stop mongod


## Add a local hostname for the certificate
Create hostnames in `/etc/hosts`. e.g. `127.0.0.1 localmongo1`

## Create certificates for CA, Server, and Client:

* [create a certificate authority certificate](https://docs.mongodb.com/manual/appendix/security/appendixA-openssl-ca/)

* [create openssl server certificate](https://docs.mongodb.com/manual/appendix/security/appendixB-openssl-server/#appendix-server-certificate)
When creating "openssl-test-server.cnf", fill in the DNS.1 and IP.1 settings with the local hostname you set up in /etc/hosts. E.g. "DNS.1 = localmongo1" and "IP.1 = 127.0.0.1".
Delete DNS.2 and IP.2.

* [create openssl client certificate](https://docs.mongodb.com/manual/appendix/security/appendixC-openssl-client/#appendix-client-certificate)

### Start mongo server with TLS, allowing invalid certificates
```
mongod -dbpath /Users/ccho/dev/drivers/test_db_data -logpath <path>/mongod.log --sslMode requireSSL --clusterAuthMode=x509 --sslPEMKeyFile <path>/test-server1.pem --sslCAFile <path>/test-ca.pem -fork
```



#### Test auth of admin user with mongosh
```
mongo --ssl --sslPEMKeyFile <path>/test-server1.pem --sslCAFile <path>/test-ca.pem --host localmongo1  --authenticationMechanism MONGODB-X509 --authenticationDatabase='$external'
```

## Additional setup for specific Drivers


### On building keystore and truststore
Required for specific drivers.

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
