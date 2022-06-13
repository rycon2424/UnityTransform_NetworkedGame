<?php
   
   include "Connection.php";

   $query = "SELECT username, score FROM UsersLogin ORDER BY convert(`score`, UNSIGNED INTEGER) DESC LIMIT 10";
   $result = $mysqli->query($query);

    while($row = mysqli_fetch_assoc($result))
    {
        $posts[] = $row;
    }

    $x = 0;
     for ($x = 1; $x < 11; $x++) 
     {
               $myObj->name = $posts[$x - 1];
               //$myObj->score = $posts[$x];
               $myJson = json_encode($myObj);
               echo $myJson;
     }

?>