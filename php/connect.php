<?php
$database = 'test';

$manager = new MongoDB\Driver\Manager(
  "mongodb://CN=ChrisChoClient,OU=TestClientCertificateOrgUnit,O=TestClientCertificateOrg,L=TestClientCertificateLocality,ST=TestClientCertificateState,C=US@localmongo1/?ssl=true&authMechanism=MONGODB-X509",
  [],
  [
    "pem_file" => "/Users/ccho/dev/drivers/certs/v3/test-client.pem",
    "ca_file" => "/Users/ccho/dev/drivers/certs/v3/test-ca.pem",
  ]
);


$bulk = new MongoDB\Driver\BulkWrite;
$bulk->insert(['phptest' => 'ok']);

$manager->executeBulkWrite('test.stuff', $bulk);
