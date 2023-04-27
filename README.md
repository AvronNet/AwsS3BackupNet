
# AwsS3LifeBackup

This is a test project for AWS S3 integration into a .NET 7 Web API application.

# Set up AWS credentials

Steps:
1. install AWS CLI client on your machine
2. test if aws command exists by running
	`aws --version`
3. in AWS console in IAM add a user
	- create a user group with the appropriate access to S3
	- if adding all permission - under the group privilages add AmazonS3FullAccess
4. Open your IAM User and under Access Keys click Create Access Key
	- you will get an Access key ID and and Secret key
5. In the console run 
	`aws configure`
	This will ask for the above key ID and secret key.
	It will ask you for a region you want to use, enter the region alias.
	The last question you can leave blank.
6. This created a credentials file in your user folder
	- on Windows this is C:\Users\[username]\.aws
7. edit this file and replace [default] with the AWS profile name set your appsettings.json file of the startup project