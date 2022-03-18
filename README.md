## Info
TCDev CSW is a service that wraps cloud storage providers into a single API layout and a single point of contact. 
You only need a valid API Key for each and can access Dropbox, OneDrive and Sharepoint by using the same API Scheme and you only
have to implement one API in your product to integrate all three services. 

We're working on adding more providers soon. 


## Getting Started

### Docker
```
docker run -e -p 5000:80 -d deejaytc/tcdev-csw
```

## Current Status

| Function | SharePoint | Dropbox | OneDrive | OneDrive Business | Box.com | GDrive 
|----------|----------|----------|----------|----------|----------|----------|
| Account Info  | 100%  | 100%      |   100%    | 100%      |  no    | no     |
| Get and search folder content  | 100%  | 50%     |   100%    | 100%     |  no    | no     |
| Upload files change file content etc  | 100%  | 50%      |   80%    | 80%     |  no    | no     |
| Delete files, file versioning  | 100% | 50%      |   100%    | 100%     | no    | no     |




### Spec here:

