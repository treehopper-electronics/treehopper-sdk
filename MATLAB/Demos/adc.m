import Treehopper.*;

board.Pins.Item(0).Mode = PinMode.AnalogInput;

data = zeros(1000, 1);

for i=1:length(data)
    data(i) = board.Pins.Item(0).AwaitAnalogVoltageChange.Result;
end

plot(data);