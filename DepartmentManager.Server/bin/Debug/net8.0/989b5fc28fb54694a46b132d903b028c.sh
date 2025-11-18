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

ps 10935;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 10935 > /dev/null;
done;

for child in $(list_child_processes 10936);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/pranayreddy/Downloads/DepartmentManager-master/DepartmentManager.Server/bin/Debug/net8.0/989b5fc28fb54694a46b132d903b028c.sh;
