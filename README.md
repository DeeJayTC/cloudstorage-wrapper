## Info
TCDev CSW is a microservice that wraps cloud storage providers into a single API layout and a single point of contact. 
You only need a valid API Key for each and can access Dropbox, OneDrive and Sharepoint by using the same API Scheme and you only
have to implement one API in your product to integrate all three services. 

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
* provider -> either dropbox, onedrive or sharepoint
* accessToken -> the coresponding access token for the provider

The services does NOT handle authentication, you need to have a valid token to use it so you need to do the oauth token handling
yourself. 
