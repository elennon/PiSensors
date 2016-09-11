<?php

include 'sdp610.php'; 
// capture command line arguments
$inputArg = $argv[1]; #argument 0 is file name.

//call function in MainFile
$output = MainFile;

//write back to the commandline
echo $output; 

?>