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

ps 7251;
while [ $? -eq 0 ];
do
  sleep 1;
  ps 7251 > /dev/null;
done;

for child in $(list_child_processes 7255);
do
  echo killing $child;
  kill -s KILL $child;
done;
rm /Users/pranayreddy/Downloads/DepartmentManager-master/DepartmentManager.Server/bin/Debug/net8.0/ec07630065534dbe85bf9aabb6fe1e7d.sh;
