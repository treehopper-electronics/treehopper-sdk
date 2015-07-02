% Treehopper MATLAB blink function

Treehopper('open');         % open the first board available
Treehopper('digitalOut',1); % make pin 1 a digital output
val = false(1);
while true(1)
    Treehopper('digitalWrite', 1, val);
    val = ~val;
    pause(1);
end