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

ps 7069;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 7069 > /dev/null;
done;

for child in $(list_child_processes 7072);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/reynald/My-Programs/DotnetCore/core8_nextjs_postgre/bin/Debug/net8.0/dbc17b3120714ef0bf183d7b5cf6ec4f.sh;
