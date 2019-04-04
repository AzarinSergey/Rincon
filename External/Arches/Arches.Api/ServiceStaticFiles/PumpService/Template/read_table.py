import csv
import datetime

EPSILON = 1.0e-14
def IS_ZERO(x):
    return x < EPSILON and -x > -EPSILON

def LESS_OR_EQ(a, b):
    return a < b or IS_ZERO(a-b)

class Table_reader:
    def __init__(self, file_obj_Vx, file_obj_ECN):
        print(file_obj_Vx)
        self.file_obj_Vx = file_obj_Vx
        self.file_obj_ECN = file_obj_ECN
        self.input_time_list = []
        self.input_Pin_list = []
        self.input_Vx = []
        self.input_frequency = []
        self.input_temp = []
        self.pressure = []
        self.frequency = []
        self.temp = []
        self.csv_reader_Vx_file()
        self.csv_reader_ECN_file()
        self.correct_time()
        self.set_date()

    def csv_reader_Vx_file(self):
        reader = csv.reader(self.file_obj_Vx, delimiter=';', quotechar=' ')
        i = 0
        for row in reader:
            work_list_time = row[1].split()
            work_list_Vx = row[26].split()
            if i > 0:
                work_list_time.pop(0)
                self.input_time_list.append(work_list_time)
                self.input_Vx.append(work_list_Vx)
            i+=1

    def csv_reader_ECN_file(self):
        reader = csv.reader(self.file_obj_ECN, delimiter=';', quotechar=' ')
        i = 0
        for row in reader:
            work_list_frequency = row[9].split()
            work_list_Pin = row[10].split()
            work_list_temp = row[12].split()
            if i > 0:
                self.input_Pin_list.append(work_list_Pin)
                self.input_frequency.append(work_list_frequency)
                self.input_temp.append(work_list_temp)
            i+=1

    def correct_time(self):
        time_arr = []
        for time in self.input_time_list:
            t_obj = datetime.time(int(str(time)[2:4]), int(str(time)[5:7]), int(str(time)[8:10]))
            time_arr.append((t_obj.hour * 60 + t_obj.minute) * 60 + t_obj.second)
        self.input_time_list = time_arr

    def total_time(self):
        return self.input_time_list[len(self.input_time_list) - 1] - self.input_time_list[0]

    def set_date(self):
        Vx_size = len(self.input_Vx) - 1
        self.pressure = [[0] * 2 for i in range(Vx_size)]
        self.frequency = [[0] * 2 for i in range(Vx_size)]
        self.temp = [[0] * 2 for i in range(Vx_size)]

        self.pressure[0][1] = self.input_Pin_list[(int(self.input_Vx[0][0]) - 1)][0]
        self.pressure[0][1] = float(self.pressure[0][1].replace(',', '.'))

        self.frequency[0][1] = self.input_frequency[(int(self.input_Vx[0][0]) - 1)][0]
        self.frequency[0][1] = float(self.frequency[0][1].replace(',', '.'))

        self.temp[0][1] = self.input_temp[(int(self.input_Vx[0][0]) - 1)][0]
        self.temp[0][1] = float(self.temp[0][1].replace(',', '.'))

        for i in range(1, Vx_size):
            self.pressure[i][0] = self.pressure[i - 1][0] + (self.input_time_list[i] - self.input_time_list[i - 1])
            self.pressure[i][1] = self.input_Pin_list[5 * i + (int(self.input_Vx[i][0]) - 1)][0]
            self.pressure[i][1] = float(self.pressure[i][1].replace(',','.'))

            self.frequency[i][0] = self.frequency[i - 1][0] + (self.input_time_list[i] - self.input_time_list[i - 1])
            self.frequency[i][1] = self.input_frequency[5 * i + (int(self.input_Vx[i][0]) - 1)][0]
            self.frequency[i][1] = float(self.frequency[i][1].replace(',','.'))

            self.temp[i][0] = self.temp[i - 1][0] + (self.input_time_list[i] - self.input_time_list[i - 1])
            self.temp[i][1] = self.input_temp[5 * i + (int(self.input_Vx[i][0]) - 1)][0]
            self.temp[i][1] = float(self.temp[i][1].replace(',','.'))


    def search(self, values, time):
        size = len(values)

        if LESS_OR_EQ(time, values[0][0]):
            return values[0][1]

        if LESS_OR_EQ(values[size - 1][0], time):
            return values[size - 1][1]

        time_lower = 0
        time_upper = 0
        val_lower = 0
        val_upper = 0

        for i in range(size):
            if LESS_OR_EQ(values[i][0], time):
                time_lower = values[i][0]
                val_lower = values[i][1]

            if LESS_OR_EQ(time, values[i][0]):
                time_upper = values[i][0]
                val_upper = values[i][1]
                break

        y = 0

        if not IS_ZERO(time_upper - time_lower):
            y = val_lower + (val_upper - val_lower) / (time_upper - time_lower) * (time - time_lower)
        else:
            y = val_lower

        return y

    def get_pressure(self, time):
        return self.search(self.pressure,time) * 1e6

    def get_frequency(self, time):
        return self.search(self.frequency,time)

    def get_temperature(self,time):
        return 273.0 + self.search(self.temp,time)


if __name__ == "__main__":
    with open("Vx.csv") as Vx_obj:
        with open("ECN_VR.csv") as ECN_obj:
            out_reader = Table_reader(Vx_obj,ECN_obj)
            a = out_reader.get_pressure(0.01)
            print(out_reader.total_time())

        '''         # print(out_reader.total_time())
           out_reader.set_date()
           print(out_reader.frequency)
        print(out_reader.Vx)
           print(out_reader.Pin_list)
           print(out_reader.frequency)
'''
