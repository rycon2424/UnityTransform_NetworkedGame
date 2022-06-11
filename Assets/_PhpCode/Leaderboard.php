<?php
   include "Connection.php";

   $query = "SELECT username, score FROM UsersLogin ORDER BY convert(`score`, UNSIGNED INTEGER) DESC LIMIT 10";
   $result = $mysqli->query($query);

    while($row = mysqli_fetch_assoc($result))
    {
        $posts[] = $row;
    }

     foreach ($posts as $row) 
      { 
            foreach ($row as $element)
            {
                echo $element."@";
            }
      }

?>