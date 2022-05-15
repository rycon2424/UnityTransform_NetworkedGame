<?php

   include "Connection.php";

   $username=$_POST["name"];
   $password=$_POST["password"];

   $namecheckquery= "SELECT username, salt, hash, score FROM UsersLogin WHERE username= '$username';";

   $namecheck = mysqli_query($mysqli, $namecheckquery) or die(" ERROR CODE 2: namecheck failed");
    if (mysqli_num_rows($namecheck) != 1)
    {
        echo" ERROR CODE 3:does not exist";
        exit();
    }

   //get login info from query 
   $existinginfo = mysqli_fetch_assoc($namecheck); 
   $salt = $existinginfo["salt"];
   $hash = $existinginfo["hash"];
   
   $loginhash = crypt($password, $salt); 
   if ($hash != $loginhash)
   {
       echo "6: Incorrect password for" . " - " . $username. " - "; //error code #6 - password does not hash to match table 
       exit();
   }

    echo $existinginfo["score"];

?>

