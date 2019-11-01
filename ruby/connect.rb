require 'mongo'

begin
  client = Mongo::Client.new( 
                             ['localmongo1:27017'],
                             auth_mech: :mongodb_x509,
                             ssl: true,
                             ssl_cert:    '/Users/ccho/dev/x509-driver-snippets/certs/test-client.pem',
                             ssl_key:    '/Users/ccho/dev/x509-driver-snippets/certs/test-client.pem',
                             ssl_ca_cert: '/Users/ccho/dev/x509-driver-snippets/certs/test-ca.pem',

                            )
                             #ssl_verify: false,
                             #tlsInsecure: true)

  puts Thread.current.backtrace

  db = client.database[:test]
  collection = client[:stuff]

  doc = { name: 'Ruby', hobbies: [ 'hiking', 'tennis', 'fly fishing' ] }

  result = collection.insert_one(doc)
  puts result.n
rescue StandardError => e
  puts e.backtrace
end

