# CodingChallenge

#steps to debug this app via a docker.

1. Pre-requisites
    * Visual Studio 2022
    * .Net Core 7.0 or higher must be installed
    * Docker Desktop

1. Please download a docker (if not installed yet) from this url: https://www.docker.com/products/docker-desktop/
2. Run it as administrator.
3. Open Visual studio (as a admin) and select "Report.WebApi" project as startup.
4. Build the solution (this would create an image/container for the docker).
5. To start debugging, set to debug mode and the hosting must be "Container (Dockerfile)", then press F5.
4. Visit https://localhost:32774/swagger/index.html.


#steps to check if all unit tests are passed
1. Goto Test menu
2. "Run All Tests"
3. Open "Test Explorer", all test must be in green color / passed.


#steps to test the API / generation of report
1. please open "files" folder on the same directory
2. upload reports.json and template.txt to the swagger UI
    2.1 a success response is expected and file can be downloaded with "reports.txt" as a filename.
3. the for testing, you can upload any files.
