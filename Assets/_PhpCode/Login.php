<?php

   include "Connection.php";

   $username=$_POST["name"];
   $password=$_POST["password"];

   if (!filter_var($username, FILTER_SANITIZE_STRING) || !filter_var($password, FILTER_SANITIZE_URL))
   {
      echo"ERROR CODE 9: Use of illigal characters detected";
      exit();
   }

   $namecheckquery= "SELECT username, salt, hash, score FROM UsersLogin WHERE username= '$username';";

   $namecheck = mysqli_query($mysqli, $namecheckquery) or die(" ERROR CODE 2: namecheck failed DATABASE Error");
    if (mysqli_num_rows($namecheck) != 1)
    {
        echo" ERROR CODE 6: User does not exist or entered wrong password";
        exit();
    }

   //get login info from query 
   $existinginfo = mysqli_fetch_assoc($namecheck); 
   $salt = $existinginfo["salt"];
   $hash = $existinginfo["hash"];
   
   $loginhash = crypt($password, $salt); 
   if ($hash != $loginhash)
   {
       echo" ERROR CODE 6: User does not exist or entered wrong password";
       exit();
   }
    echo "1";
    echo $username;
    echo $existinginfo["score"];

?>

