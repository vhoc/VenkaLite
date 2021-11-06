# VenkaLite.
A simple program to update Venka's API with data from the customer's SoftRestaurant database.
---------------------------
Made by:  
Victor H. Olvera.  
Venka Development Team.
---------------------------
First draft completed on May 11st, 2021.  
Rewritten: November 1st, 2021
---------------------------

First time run:

Run the program as root in Linux, or Administrator in Windows with *--config* parameter and enter the following fields:
- ID of the restaurant
- ID of the database type (1 for SQLSERVER)
- Database instance (COMPUTER-NAME\SQLSERVERINSTANCE)
- Database name (softrestaurant95pro)
- Database username
- Database password

When installing on a new venka-nanoserver image, run **crontab -e** then save the crontab.
Then, run the script **cronjob.sh** as a normal user. This will make VenkaLite run every 1 minute which can be changed running **crontab -e** again and change the 
corresponding value.
