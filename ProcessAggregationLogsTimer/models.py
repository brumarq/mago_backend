class DeviceLogValueStats:
    def __init__(self):
        self.min = 0.0
        self.max = 0.0
        self.sum = 0.0
        self.count = 0

# Retrieves log value statistics for a specific field. If no field, create new entry with the field_id and get log info
class DeviceFieldStats(dict):
    def get_field_stats(self, field_id):
        if field_id not in self:
            self[field_id] = DeviceLogValueStats()
        return self[field_id]

# Retrieves statistics for specified device and field. If no device, create new entry with the device_id and get field info
class DeviceStats(dict):
    def get_device_stats(self, device_id, field_id):
        if device_id not in self:
            self[device_id] = DeviceFieldStats()
        return self[device_id].get_field_stats(field_id)