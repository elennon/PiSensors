<?php

$n=0;
//while(1){
	$cmd="sudo i2cset -y 1 0x40 0xf1";
	$result=`$cmd`;
		
	$haveMeasurement=false;
	$hexStr="";
	while($haveMeasurement==false){	
		$cmd="sudo i2cdump -y 1 0x40 W | tail -n 1";
		$result=`$cmd`;

		$s1=preg_split("/\s+/",$result);
		$hexStr=$s1[11].$s1[12];
		
		if($hexStr!="XXXX"){
			$haveMeasurement=true;
		}
		sleep(0.1);
	}
	$val=hexdec($hexStr);

	$val=$val - (2^16/2);

	$calibrationParameter = 16;
	
	$val = ($val/60)*(96600/102029)-500-$calibrationParameter;

	echo time()."\n";
	echo $val;
	echo "\n";
	
	//sleep(1);
//}
?>
