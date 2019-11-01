local  sleeper_lua_config = {
[ 1000 ]={  id = 1000, name = [==[pigman]==], sleepcondition =  { time={3}, home=0,fight=0, burnable=0, leader= 0 } , wakecondition =  { time={1},fight=1, burnable=1 } , resistance = 3, sleepiness = 0,},
[ 1001 ]={  id = 1001, name = [==[werepig]==], resistance = 3, sleepiness = 0,},
[ 1108 ]={  id = 1108, name = [==[moonpig]==], resistance = 3, sleepiness = 0,},
[ 1018 ]={  id = 1018, name = [==[spider]==], sleepcondition =  { time={1},home=0, fight=0, burnable=0, leader= 0 } , wakecondition =  { time={2,3}, fight=1, burnable=1, leader=1  } , resistance = 2, sleepiness = 0,},
}

return  sleeper_lua_config