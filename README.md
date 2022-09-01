# RocketSystemProjectTemplate

Provides a working template for creating new RocketSystem Projects.

1. Replace "rocketsystemprojecttemplate" with "newprojectname" case-sensitive in Project 
2. Replace "RocketSystemProjectTemplate" with "NewProjectName" case-sensitive in Project 
3. Rename Project files
4. Rename Solution files
5. Rename Resx File.
6. Rename DNN manifest.

The system uses the RocketPortal>RocketAdmin skin on the admin page which will call the AdminPanel.  

DO NOT FORGET:  Add any new commands into the "SystemDefaults.rules" file.

#### Installation

The first step after a successful build is to install the installation package.  This can be found in the "\Installation" folder after the project has been compiled in release mode.  
The install zip can be installed in the test system by selecting "CMS Admin" in the top left mennu and then select "extensions".

##### Menu
The side menu shows interfaces.  You need to create (Copy/Paste) an interface WITH a "default command", so the security works correctly.  Ensure you have added the command to the "SesysmDefaults.rules" file. 


The current project code uses DNNpacker.

https://github.com/leedavi/DNNpackager

This can be removed from the Build Events, if it is not required.
