## Info
TCDev CSW is a microservice that wraps cloud storage providers into a single API layout and a single point of contact. 
You only need a valid API Key for each and can access Dropbox, OneDrive and SharePoint by using the same API Scheme and you only
have to implement one API in your product to integrate all three services. It was developed for Teamwork.com and is still being used today.

If you run a SAAS app and want to implement either of the three, why don't you just implement all three at once? :)

We're working on adding more providers soon. 


## Getting Started

### Docker
```
docker run -e -p 5000:80 -d deejaytc/tcdev-csw
```

When the service is running you can access the open api spec via https://localhost:80/swagger

### Pull
* Pull the code
* Run it :)

## Usage

To call the api you need to pass two header values

For provider selection
```
provider: dropbox | onedrive | sharepoint
```

For Authorization
```
either accessToken: <yourToken>
or     authorization: bearer <yourToken>
```

The services does NOT handle authentication, you need to have a valid token to use it so you need to do the oauth token handling
yourself. 
