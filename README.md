# x509-driver-snippets
Connection code snippets for MongoDB drivers using x.509 authentication

# Setup

## Start mongod without TLS
https://www.mongodb.com/docs/manual/tutorial/manage-mongodb-processes/#start-mongod-processes

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

Record your user values since your client certificate will need to match them.

## Stop mongod
https://www.mongodb.com/docs/manual/tutorial/manage-mongodb-processes/#stop-mongod-processes

## Add a local hostname for the certificate
Create hostnames in `/etc/hosts`. e.g. `127.0.0.1 localmongo1`

## Create certificates for CA, Server, and Client:

* [create a certificate authority certificate](https://docs.mongodb.com/manual/appendix/security/appendixA-openssl-ca/)

* [create openssl server certificate](https://docs.mongodb.com/manual/appendix/security/appendixB-openssl-server/#appendix-server-certificate)
When creating "openssl-test-server.cnf", fill in the DNS.1 and IP.1 settings with the local hostname you set up in /etc/hosts. E.g. "DNS.1 = localmongo1" and "IP.1 = 127.0.0.1".
Delete DNS.2 and IP.2.
Make sure all the certificate values match the client certificate / user values except for either the Organization (O) or Organization Unit (OU).

* [create openssl client certificate](https://docs.mongodb.com/manual/appendix/security/appendixC-openssl-client/#appendix-client-certificate)


### Start mongo server with TLS, allowing invalid certificates
```
mongod --dbpath=/Users/chris.cho/dev/mongo_data/6.0.0 --replSet \"myRS\" --tlsMode requireTLS --tlsCertificateKeyFile test-server1.pem  --tlsCAFile test-ca.pem --bind_ip localmongo1 --tlsAllowInvalidCertificates
```


#### Test auth of admin user with mongosh
```
mongosh --tls --host localmongo1 --tlsCertificateKeyFile test-client.pem  --tlsCAFile test-ca.pem --authenticationMechanism MONGODB-X509 --authenticationDatabase='$external' --tlsAllowInvalidCertificates
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
