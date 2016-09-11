<?php

$returnObj=array();
$returnObj['ok']=0;
$returnObj['msg']="NOK";
$returnObj['sensor']="ds18b20";


try{
	$cmd="cat /sys/bus/w1/devices/28-00*/w1_slave 2> /dev/null";
	$result=`$cmd`;

	$s1=preg_split("/\n/",$result);
	foreach($s1 as $elem){
		$s2=preg_split("/t=/",$elem);
		if(sizeof($s2)>1){
			if(is_numeric($s2[1])){
			$returnObj['ok']=1;
			$returnObj['msg']="OK";
			$returnObj['data']=$s2[1]/1000;
			$returnObj['time']=time()+microtime();
			}
		}
	}
}catch(Exception $e){
	//do nothing
}

echo json_encode($returnObj)."\n";
?>
