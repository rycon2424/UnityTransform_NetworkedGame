<?php

   include "Connection.php";

   $username=$_POST["name"];
   $password=$_POST["password"];
   $namecheckquery= "SELECT username FROM UsersLogin WHERE username= '$username';";

   //echo "Searched for a duplicate of " . $username .  $namecheckquery;
   $namecheck = mysqli_query($mysqli,$namecheckquery) or die(" ERROR CODE 2: namecheck failed");

    if (mysqli_num_rows($namecheck) > 0)
    {
        echo" ERROR CODE 3:name exist";
        exit();
    }

    $salt="\$5\$rounds=5000\$"."BoskoEncryption".$username."\$";
    $hash=crypt($password,$salt);
    $insertuserquery="INSERT INTO UsersLogin (username, hash, salt) VALUES ('$username','$hash','$salt');";
    mysqli_query($mysqli,$insertuserquery)or die(" ERROR CODE 4:insert player failed" . $mysqli -> error);

    echo("0");
?>