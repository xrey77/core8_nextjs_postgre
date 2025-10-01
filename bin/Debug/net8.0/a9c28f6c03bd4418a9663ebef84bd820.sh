function list_child_processes () {
    local ppid=$1;
    local current_children=$(pgrep -P $ppid);
    local local_child;
    if [ $? -eq 0 ];
    then
        for current_child in $current_children
        do
          local_child=$current_child;
          list_child_processes $local_child;
          echo $local_child;
        done;
    else
      return 0;
    fi;
}

ps 24859;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 24859 > /dev/null;
done;

for child in $(list_child_processes 24860);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/reynald/My-Programs/DotnetCore/core8_nextjs_postgre/bin/Debug/net8.0/a9c28f6c03bd4418a9663ebef84bd820.sh;
