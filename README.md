# RESO-WebAPI-Client-dotNet

Open Source .Net Code Based RESO Web API Reference Client for accessing RESO Web API Servers for Real Estate Data. More information at http://www.reso.org

## Requirements

Windows Visual Studio 2017 C#.  Latest build using .NET Framework 4.6.1.

## Getting Started and Installation

Download the source code.  The primary solution file is RESOReference.sln.  Open the solution file and build the project.

RESOReference is the primary desktop application.
RESOClientLibrary contains the code to perform the http requests for the primary application.
ReferenceLibrary is a library of helper classes to support various tests.
ODataValidator.CodeRules is a version of the OData tests that are performed.  It has been modified to pass header information correctly to support OAuth authorization headers correctly.  If you have errors with your server please set a break in the Verify function of the test to debug the error.
ODataTransactions is currently being worked on.  It will support a closer examination of the metdata.



## Dependencies

Requires Newtonsoft.Json version 9.01.


## Specific code configuration, usage guidelines, depending on situation

There are various ways to use the client.  When testing your server you can use the existing OData tests by using the Run Validation Test button or you can generate your own OData requests and run them using the Execute Test Script button.

In the bin\Debug\config directory you have the RuleControl.xml file.  You can limit which OData rule you want executed by removing the tests you do not want executed.
An example of testing a single rule is in rulecontrol.testrule.  Each of the XML nodes represent results that can be overridden in your output.

The TestScript has 3 sections.
ClientSettings -  Files out the GUI on the reference client.  The following attributes can be set:
    WebAPIURI
    AuthorizationURI
    TokenURI
    RedirectURI
    AuthenticationType
    BearerToken
    ClientIdentification
    ClientSecret
    UserName
    Password
    ClientScope
    Version
    Preauthenticate
    TransactionLogDirectory
    ResultsDirectory
    ScriptFile

Parameters -  You can set parameters that are used in the requests.  It has two attributes Name and Value.
Requests - The requests made to the server and where they are outputed.  It has two attributes OutputFile and URL.  The paramenter name is bracketed by * to denote a replacement with the value.

## Contributing

Please read the [contributing guidelines](CONTRIBUTING.md) if You are interested in contributing to the project.

## Authors

* **Stuart Schuessler**

See also the list of [contributors](https://github.com/RESOStandards/RESO-WebAPI-Client-dotNet/graphs/contributors) who participated in this project.

## License

This project is licensed under the RESO License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Real Estate Standards Organization
* National Association of REALTORS


[![Real Estate Standards Organziation](https://www.reso.org/wp-content/uploads/2016/10/RESO.png)](https://reso.org)
