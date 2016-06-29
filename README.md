# Display Groups

This project is a code sample to show how to list all groups and membership of a Team Foundation Server (TFS) or Visual Studio Team Services (VSTS) collection.

This is a command-line application, and the output can be redirected to a text file.

## Example usage:<br/>
DisplayGroups.exe https://myserver.visualstudio.com/DefaultCollection<br/>

To redirect the output to a text file:<br/>
DisplayGroups.exe https://myserver.visualstudio.com/DefaultCollection >GroupsAndMembers.txt

Here is an example of the output that you would see:

Members of group: [DefaultCollection]\Project Collection Administrators<br/>
***
vstfs:///Framework/IdentityDomain/bzz2ec97-f70a-4374-9037-9492666f000f\Team Foundation Administrators<br/>
vstfs:///Framework/Generic/dd9075cf-d538-43d5-807f-3c7dba3a9k52\Project Collection Service Accounts<br/>
wredford@contoso.com<br/>
scot@contoso.com<br/>
LOCAL AUTHORITY\TeamFoundationService (TEAM FOUNDATION)<br/>
daka@contoso.com<br/>
bkel@contoso.com<br/>

Members of group: [TEAM FOUNDATION]\Team Foundation Invited Users<br/>
***
wredford@contoso.com<br/>
scot@contoso.com<br/>
daka@contoso.com<br/>
chris@contoso.com<br/>
sastry@contoso.com<br/>
abrk@contoso.com<br/>
bkel@contoso.com<br/>
