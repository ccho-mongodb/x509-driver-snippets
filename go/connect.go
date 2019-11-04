package main

import (
    "context"
    "fmt"
    "log"
    "go.mongodb.org/mongo-driver/mongo"
    "go.mongodb.org/mongo-driver/mongo/options"
    "go.mongodb.org/mongo-driver/mongo/readpref"
)


// You will be using this Trainer type later in the program
type Trainer struct {
    Name string
    Age  int
    City string
}

func main() {

    uri := "mongodb://localmongo1:27017/?authMechanism=MONGODB-X509&ssl=true&sslCertificateAuthorityFile=./ca.pem&sslClientCertificateKeyFile=./client.pem"
    credential := options.Credential{
        AuthMechanism: "MONGODB-X509",
    }
    clientOpts := options.Client().ApplyURI(uri).SetAuth(credential)
    client, err := mongo.Connect(context.TODO(), clientOpts)
    if err != nil {
        log.Fatal(err)
    }
    
    fmt.Println("Connected to MongoDB!")

    // Rest of the code will go here
    collection := client.Database("test").Collection("stuff")

    rec := Trainer{"A", 10, "Town"}
    insertResult, err := collection.InsertOne(context.TODO(), rec)
    if err != nil {
        log.Fatal(err)
    }

    fmt.Println("Inserted a single document: ", insertResult.InsertedID)
}