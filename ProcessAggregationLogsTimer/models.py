class DeviceLogValueStats:
    def __init__(self):
        self.min = 0.0
        self.max = 0.0
        self.sum = 0.0
        self.count = 0

class DeviceFieldStats(dict):
    def get_field_stats(self, field_id):
        if field_id not in self:
            self[field_id] = DeviceLogValueStats()
        return self[field_id]

class DeviceStats(dict):
    def get_device_stats(self, device_id, field_id):
        if device_id not in self:
            self[device_id] = DeviceFieldStats()
        return self[device_id].get_field_stats(field_id)