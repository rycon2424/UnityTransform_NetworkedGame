<?php
//$db_user = 'boskoivkovic';
//$db_pass = 'Zeix7Ohh5c';

$db_user = $_POST["username"];
$db_pass = $_POST["password"];

if (empty($_POST["username"]) || empty($_POST["password"]))
{
    echo "Failed because user or pass is not set, error code 0";
    exit();
}

$db_host = 'localhost';
$db_name = 'boskoivkovic';

// Open a connection
$mysqli = new mysqli("$db_host","$db_user","$db_pass","$db_name");

// Check connection
if ($mysqli->connect_errno) 
{
   echo "0";
   exit();
}

// Check if server is still alive
if (!mysqli_ping($mysqli)) 
{
    echo 'Lost connection, exiting';
    exit;
}

session_start();

echo "1" . "<br>(" . "Session ID: " . session_id() . ")<br>";

function showerror($error,$errornr) 
{
die("Error (" . $errornr . ") " . $error);
}
?>