# decathlon-stock
Simple web app .Net Core MVC to find stock and send email of a product in Decathlon.es with Selenium

To use the app you need to follow two steps:

1. You need a Google Chrome browser because Selenium use it to open de url and find if the product is in stock or not.
2. Next you need a Google Gmail account without two factor, to send the email when the product is in stock. I recommend create a new account to receive the emails. When your account is created you can go to the "Less secure apps" section in Google Account and then turn on "Access for less secure apps". Put the credentials in appsettings.json.
3. Finally you can start the app and put the url and the email. Press start to commence the minute interval, and cancel to restart. If you close the app you will lose all data (the app doesn't have a DB).
