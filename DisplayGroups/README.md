# Display Groups

This project is a code sample to show how to list all groups and membership of a Team Foundation Server (TFS) or Visual Studio Online (VSO) collection.

This is a command-line application, and the output can be redirected to a text file.

## Example usage:
DisplayGroups https://myserver.visualstudio.com/DefaultCollection
To redirect the output to a text file:
DisplayGroups https://myserver.visualstudio.com/DefaultCollection >GroupsAndMembers.txt

Here is an example of the output that you would see:

Members of group: [DefaultCollection]\Project Collection Administrators
=================
vstfs:///Framework/IdentityDomain/bzz2ec97-f70a-4374-9037-9492666f000f\Team Foundation Administrators
vstfs:///Framework/Generic/dd9075cf-d538-43d5-807f-3c7dba3a9k52\Project Collection Service Accounts
wredford@contoso.com
scot@contoso.com
LOCAL AUTHORITY\TeamFoundationService (TEAM FOUNDATION)
daka@contoso.com
bkel@contoso.com
dandan@contoso.com
ridasi@contoso.com

Members of group: [TEAM FOUNDATION]\Team Foundation Invited Users
=================
wredford@contoso.com
scot@contoso.com
chris@contoso.com
sastry@contoso.com
abrk@contoso.com
mressle@contoso.com