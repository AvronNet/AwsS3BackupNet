
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


# Set up AWS S3 bucket with cloudfront access

Steps:
1. Create a bucket with the Block all public access property checked
2. Create an AWS CloudFront Distribution with properties:
	- origins - the AWS S3 bucket we want to expose
		- this will give you a prompt in AWS that you will get a permissions policy to set on your S3 bucket after creating this Distribution
	- select the possible HTTP methods that can be used on that bucket publicly
	- set other properties as you see fit
	- click Create
	- at the top of the page look for the prompt for that gives you the permission policy for the S3 bucket
		- click the copy permissions script button
		- click the link to the bucket configuration
		- in Bucket policy section click the Edit button and paste the policy configuration from your CloudFront distribution
	- DONE
3. Access your bucket files from the cloudfront URL
	- URL format - [CloudFront_Distribution_domain_name]/[S3_prefix_aka_subfolder]/[filename_with_extension]