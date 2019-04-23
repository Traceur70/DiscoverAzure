/*
Inputs integration of this function :
    COSMOS DB
        Document parameter name : "bookmark"
        database name : "<db name in cosmoss db>"
        collection name : "<coll name in cosmoss db>"
        Cosmos DB account : "<select it>"
        Document ID : "<id's column name of db in cosmos db>"
        Partition Key : "<partition key of db in cosmos db>"

Outputs integration of this function :
    COSMOS DB
        Document parameter name : "newbookmark"
        database name : "<db name in cosmoss db>"
        collection name : "<coll name in cosmoss db>"
        Cosmos DB account : "<select it>"
        Partition Key : "<partition key of db in cosmos db>"
    
    AZURE QUEUE STORAGE OUTPUT
        Message parameter name : newmessage
        Queue name : bookmarks-post-process
        Storage account connection : "<select it>"
*/


//Client body's parameter : { "id": "github", "url": "https://www.github.com" }
module.exports = function (context, req) {
    var bookmark = context.bindings.bookmark
    if(bookmark){
        context.res = {
            status: 422,
            body : ("Bookmark already exists. URL : " + bookmark.url),
            headers: { 'Content-Type': 'application/json' }
        };
    }
    else {
        var bookmarkString = JSON.stringify({ id: req.body.id, url: req.body.url });// Create a JSON string of our bookmark.
        context.bindings.newbookmark = bookmarkString;// Write this bookmark to our database.
        context.bindings.newmessage = bookmarkString;// Push this bookmark onto our queue for further processing.

        // Tell the user all is well.
        context.res = {
            status: 200,
            body : "bookmark added!",
            headers: { 'Content-Type': 'application/json' }
        };
    }
    context.done();
};