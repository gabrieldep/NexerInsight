# NexerInsight
1.2 PROBLEM
The solution now needs to be complemented with a new service/program that exposes the information via a REST API to a second system. The new service must have the ability to run in Azure. The ideal goal for the API methods that the customer wishes to use as a minimum requirement to begin are listed below. Otherwise, the API is unrestricted. There are no requirements for security in the API endpoints, because the customer wants to be able to offer the information as an open API. It is also desirable that the model and documentation are as detailed and clear as possible for the end users. The customer also wishes to be able to add on to the functionality of the API in the future.

1.3 SPECIFICATION
The API will initially have two methods.
Collect all of the measurements for one day, one sensor type, and one unit. Examples of how a call could look:
/api/v1/devices/testdevice/data/2018-09-18/temperature
getdata?deviceId=testdevice&date=2018-09-18&sensorType=temperature
Collect all data points for one unit and one day. Examples of how a call could look:
/api/v1/devices/testdevice/data/2018-09-18
getdatafordevice?deviceId=testdevice&date=2018-09-18 Returns temperature, humidity, and rainfall for the day.
