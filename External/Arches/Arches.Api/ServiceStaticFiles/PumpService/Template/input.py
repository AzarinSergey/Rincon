#импортируем симулятор гидравлики
import HyCarSim as hcs
import math
#импортируем данные считываемые с таблицы питоновским скриптом
from read_table import Table_reader

# читаем табличку с данными VX и данными, получаемыми с ЭЦНов 
with open(r"{{VxFilePath}}") as Vx_obj:
	with open(r"{{EcnFilePath}}") as ECN_obj:
		reader = Table_reader(Vx_obj, ECN_obj)
		
# задаем состав флюида- тяжелые фракции	 и их свойства. Для теста задан Нонадекан и Эйкозан	
components = [ hcs.PureComponentProperties(name = "Nonadecane_C19H40",
										   carbon_number = 4,
										   molar_mass = 0.268,
										   pres_critical = 11.64e5,
										   temp_critical = 775.6,
										   vol_critical = 0.001176,
										   acentric_factor = 0.1852,
										   heat_capacity_gas = 300.0,
										   heat_capacity_liq = 200.0,
                                           evaporation_heat = 5e5,
										   viscosity_gas = 4e-5,
										   viscosity_liq	= 4.e-3),
										  
			   hcs.PureComponentProperties(name = "Icosane_C20H42",
										   carbon_number = 4,
										   molar_mass = 0.282,
										   pres_critical = 11.64e5,
										   temp_critical = 775.6,
										   vol_critical = 0.001176,
										   acentric_factor = 0.1852,
										   heat_capacity_gas = 300.0,
										   heat_capacity_liq = 200.0,
                                           evaporation_heat = 5e5,
										   viscosity_gas = 4e-5,
										   viscosity_liq	= 4.e-3) ]

binary_coefficients = [ hcs.InteractionBinaryCoefficient(name1 = "Nonadecane_C19H40", name2 = "Icosane_C20H42", k = 0.6) ]
# указываем способ решения системы уравнений										  
solver = hcs.UpwindSolver(components, binary_coefficients)
# задаем начальные условия, используемые для инициализации свойств НКТ и ЭЦН: температуры и давления
T0 = {{Temperature}}
Tw = {{WallTemperature}}
P0 = {{Pressure}}
P = 100236

#описание геометрии НКТ
#1. Задаем ЭЦН внизу НКТ. Параметры ЭЦН считываются из файла ECN_VR.
#Давление от времени на входе и температура в насосе считываются из файла ECN_VR. Номер насоса из файла Vx.csv
# Остальные параметры - настроечные    
#2. Описываем геометрию НКТ
# Скважина состоит из горизонтального участка длиной 2.9 км 
# и вертикального участка длиной 1.1. км.
# Диаметр трубы 0,01 м. Шерооватость 1 мм. В каждой части задано по 20 расчетных ячеек. Также задано распределение фракций
solver.traverse_simple_circuit(units = [
	hcs.PumpBoundaryCondition('bc_left', pressure = lambda t: reader.get_pressure(t), temperature = T0, length_pump = 1.0,  volume_flow = 0.09, power_consumption = 20.0, pump_pressure = 8370., frequency = lambda t: reader.get_frequency(t), molar_fraction = [0.2, 0.8] ),
	hcs.Channel(name = "channel", parts = [		
		hcs.ChannelPart(
		PartName = "Part_1",
		length = 2900.0,
		ncells = 20,
		roughness = 0.001, 
		sine = math.sin(math.radians(0)),
		diameter = 0.1,
		frac_liq = 1.0,
		frac_gas = 0.0,
		pressure = P0,
		wall_temperature = Tw,
		temperature = T0,
        molar_fraction = [0.2, 0.8]        ),
		
		hcs.ChannelPart(
		PartName = "Part_2",
		length = 1000.0,
		ncells = 20,
		roughness = 0.001, 
		sine = math.sin(math.radians(90)),
		diameter = 0.1,
		frac_liq = 1.0,
		frac_gas = 0.0,
		pressure = P0,
		wall_temperature = Tw,
		temperature = T0,
        molar_fraction = [0.2, 0.8]        ),
		]),
	hcs.PressureBoundaryCondition('bc_right', pressure = P0, temperature = T0, molar_fraction = [0.2, 0.8])
]);
# запуск симуляции и указание названия выходного файла
solver.compute_all(time = reader.total_time(), dt = 1e-4, dtmax = 1.0e-2, delta = 2, path_text = 'output.tsv')