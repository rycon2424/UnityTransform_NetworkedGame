<?php
   $db_user = 'boskoivkovic';
   $db_pass = 'Zeix7Ohh5c';
   $db_host = 'localhost';
   $db_name = 'boskoivkovic';

/* Open a connection */
$mysqli = new mysqli("$db_host","$db_user","$db_pass","$db_name");

/* check connection */
if ($mysqli->connect_errno) {
   echo "Failed to connect to MySQL: (" . $mysqli->connect_errno() . ") " . $mysqli->connect_error();
   exit();
}

echo "1";

function showerror($error,$errornr) 
{
die("Error (" . $errornr . ") " . $error);
}
?>