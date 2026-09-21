#include <iostream> 
#include <vector> 
#include <cstring> 
#include <cstdint> 
#include <cstdio> 
#include <unistd.h> 
#include <syslog.h> 
#include <sys/select.h> 
#include <sys/time.h> 
#include <cmath> 
#include <libmodbus/modbus.h> 

const int MAX_DEVICES = 5;

static bool t_active[2] = { false, false };
static int ips[2] = { 0, 0 };
static int sps[2] = { 9999, 9499 };
static int err_jmps[2] = { -1, -1 };
static long long wake_times[2] = { 0, 0 };
static int t_idx = 1;
static int exec_limit = 50000;

void float_to_registers(float f, uint16_t& low_word, uint16_t& high_word) {
	uint32_t combined;
	std::memcpy(&combined, &f, 4);
	low_word = combined & 0xFFFF;
	high_word = (combined >> 16) & 0xFFFF;
}

int main() {
	openlog("helloworld", LOG_PID, LOG_USER);
	syslog(LOG_INFO, "VM OS 3.0 Baslatildi...");

	modbus_mapping_t* mb_mapping = modbus_mapping_new(0, 0, 10000, 0);
	if (mb_mapping == nullptr) {
		syslog(LOG_ERR, "Hafiza haritasi olusturulamadi!");
		return -1;
	}

	modbus_t* ctx_pc = modbus_new_tcp("0.0.0.0", 5020);
	int server_socket = modbus_tcp_listen(ctx_pc, 1);
	if (server_socket == -1) {
		syslog(LOG_ERR, "5020 portu dinlenemiyor, port mesgul!");
		modbus_free(ctx_pc);
		return -1;
	}

	fd_set refset, rdset;
	FD_ZERO(&refset);
	FD_SET(server_socket, &refset);
	int fdmax = server_socket;
	int master_socket = -1;

	time_t last_scan_times[MAX_DEVICES] = { 0 };

	while (true) {
		rdset = refset;
		struct timeval tv;

		bool is_script_running = (t_active[0] || t_active[1]);
		tv.tv_sec = is_script_running ? 0 : 1;
		tv.tv_usec = is_script_running ? 5000 : 0;

		int ret = select(fdmax + 1, &rdset, NULL, NULL, &tv);

		if (ret > 0) {
			if (FD_ISSET(server_socket, &rdset)) {
				master_socket = modbus_tcp_accept(ctx_pc, &server_socket);
				if (master_socket >= 0) {
					FD_SET(master_socket, &refset);
					if (master_socket > fdmax) fdmax = master_socket;
					syslog(LOG_INFO, "PC Arayuzu Baglanti Kurdu.");
				}
			}
			else if (master_socket >= 0 && FD_ISSET(master_socket, &rdset)) {
				uint8_t query[MODBUS_TCP_MAX_ADU_LENGTH];
				int rc = modbus_receive(ctx_pc, query);
				if (rc > 0) {
					modbus_reply(ctx_pc, query, rc, mb_mapping);
				}
				else if (rc == -1) {
					close(master_socket);
					FD_CLR(master_socket, &refset);
					master_socket = -1;
					syslog(LOG_INFO, "PC Arayuzu Baglantiyi Kesti.");
				}
			}
		}

		static time_t last_systick_time = 0;
		time_t now = time(NULL);
		if (now != last_systick_time) {
			mb_mapping->tab_registers[8999]++;
			last_systick_time = now;
		}

		uint16_t script_trigger = mb_mapping->tab_registers[8500];

		if (script_trigger == 1) {
			t_active[0] = true; t_active[1] = false;
			ips[0] = 0; ips[1] = 0;
			sps[0] = 9999; sps[1] = 9499;
			err_jmps[0] = -1; err_jmps[1] = -1;
			wake_times[0] = 0; wake_times[1] = 0;
			t_idx = 1;
			exec_limit = 50000;

			mb_mapping->tab_registers[8500] = 2;

			const char* log_str = "MAKRO OK: Master Routing Devrede!";
			for (int k = 0; k < 30; k++) mb_mapping->tab_registers[8501 + k] = 0;
			for (size_t k = 0; k < strlen(log_str); k++) {
				if (k % 2 == 0) mb_mapping->tab_registers[8501 + (k / 2)] = (log_str[k] << 8);
				else mb_mapping->tab_registers[8501 + (k / 2)] |= log_str[k];
			}
		}

		if (t_active[0] || t_active[1]) {

			uint16_t cmd_count = mb_mapping->tab_registers[8000];
			if (cmd_count > 100) cmd_count = 100;

			bool error_flag = false;
			int chunk_steps = 150;

#define TRIGGER_ERROR(err_code) { \ 
			if (err_jmps[t_idx] != -1) {
				\
					ips[t_idx] = err_jmps[t_idx]; \
					err_jmps[t_idx] = -1; \
					mb_mapping->tab_registers[8498] = err_code; \
					continue; \
			}
			else {
				\
					error_flag = true; \
					break; \
			} \
			}

			while ((t_active[0] || t_active[1]) && exec_limit > 0 && chunk_steps > 0) {
				t_idx = (t_idx + 1) % 2;
				if (!t_active[t_idx]) continue;

				struct timeval current_tv;
				gettimeofday(&current_tv, NULL);
				long long current_ms = current_tv.tv_sec * 1000LL + current_tv.tv_usec / 1000;

				if (current_ms < wake_times[t_idx]) {
					if (!t_active[(t_idx + 1) % 2] || current_ms < wake_times[(t_idx + 1) % 2]) {
						break;
					}
					continue;
				}

				if (ips[t_idx] >= cmd_count) {
					t_active[t_idx] = false;
					continue;
				}

				int base_idx = 8001 + (ips[t_idx] * 4);
				uint16_t opcode = mb_mapping->tab_registers[base_idx];
				uint16_t src1 = mb_mapping->tab_registers[base_idx + 1];
				uint16_t src2 = mb_mapping->tab_registers[base_idx + 2];
				uint16_t dest = mb_mapping->tab_registers[base_idx + 3];

				if (src1 < 10000 && src2 < 10000 && dest < 10000) {

					if (opcode == 80) { ips[t_idx] = dest; exec_limit--; chunk_steps--; continue; }

					else if (opcode == 81) {
						if
							(mb_mapping->tab_registers[src1] == 0) {
							ips[t_idx] = dest;
							exec_limit--; chunk_steps--; continue;
						}
					}

					else if (opcode == 82) {
						if
							(mb_mapping->tab_registers[src1] != 0) {
							ips[t_idx] = dest;
							exec_limit--; chunk_steps--; continue;
						}
					}
					else if (opcode == 85) { err_jmps[t_idx] = dest; }
					else if (opcode == 90) {
						wake_times[t_idx] = current_ms + dest;
						exec_limit += 100;
					}

					// --- THREAD BİRİMİ --- 
					else if (opcode == 95) {
						ips[1] = dest; sps[1] = 9499; err_jmps[1] = -1; wake_times[1] = 0; t_active[1] = true;
					}
					else if (opcode == 96) { t_active[t_idx] = false; continue; }

					// --- FONKSİYON VE STACK --- 
					else if (opcode == 70) {
						if (sps[t_idx] > (t_idx == 0 ? 9500 : 9000)) {
							mb_mapping->tab_registers[sps[t_idx]--] = ips[t_idx] + 1;
							ips[t_idx] = dest;
							exec_limit--; chunk_steps--; continue;
						}
						else TRIGGER_ERROR(4);
					}
					else if (opcode == 71) {
						if (sps[t_idx] < (t_idx == 0 ? 9999 : 9499)) {
							ips[t_idx] = mb_mapping->tab_registers[++sps[t_idx]];
							exec_limit--; chunk_steps--; continue;
						}
						else TRIGGER_ERROR(4);
					}
					else if (opcode == 72) {

						if (sps[t_idx] > (t_idx == 0 ? 9500 : 9000))
							mb_mapping->tab_registers[sps[t_idx]--] =
							mb_mapping->tab_registers[src1];
						else TRIGGER_ERROR(4);
					}
					else if (opcode == 73) {

						if (sps[t_idx] < (t_idx == 0 ? 9999 : 9499))
							mb_mapping->tab_registers[dest] =
							mb_mapping->tab_registers[++sps[t_idx]];
						else TRIGGER_ERROR(4);
					}

					// ======================================================== 
					// MODBUS MASTER (OKUMA YAZMA YÖNLENDİRMESİ) 
					// ======================================================== 
					else if (opcode == 110) {
						uint16_t device_id = mb_mapping->tab_registers[src1];
						uint16_t remote_addr = mb_mapping->tab_registers[src2];
						if (device_id < MAX_DEVICES) {
							int base_conf = 1001 + (device_id * 6);
							uint16_t is_active = mb_mapping->tab_registers[base_conf + 5];
							if (is_active == 1) {
								uint16_t ip_high = mb_mapping->tab_registers[base_conf + 0];
								uint16_t ip_low = mb_mapping->tab_registers[base_conf + 1];
								uint16_t port = mb_mapping->tab_registers[base_conf + 2];
								uint16_t slave_id = mb_mapping->tab_registers[base_conf + 3];
								char ip_str[32];

								sprintf(ip_str, "%d.%d.%d.%d", (ip_high
									>> 8) & 0xFF, ip_high & 0xFF, (ip_low >> 8) &
									0xFF, ip_low & 0xFF);

								modbus_t* ctx_dev = modbus_new_tcp(ip_str, port);
								if (ctx_dev) {
									modbus_set_slave(ctx_dev, slave_id);
									modbus_set_response_timeout(ctx_dev, 0, 500000);
									if (modbus_connect(ctx_dev) != -1) {
										uint16_t val = 0;
										if (modbus_read_registers(ctx_dev, remote_addr, 1, &val) != -1) {
											mb_mapping->tab_registers[dest] = val;
										}
										else TRIGGER_ERROR(6);
										modbus_close(ctx_dev);
									}
									else TRIGGER_ERROR(6);
									modbus_free(ctx_dev);
								}
								else TRIGGER_ERROR(6);
							}
							else TRIGGER_ERROR(6);
						}
						else TRIGGER_ERROR(6);
					}
					else if (opcode == 111) {
						uint16_t device_id = mb_mapping->tab_registers[src1];
						uint16_t remote_addr = mb_mapping->tab_registers[src2];
						uint16_t value = mb_mapping->tab_registers[dest];
						if (device_id < MAX_DEVICES) {
							int base_conf = 1001 + (device_id * 6);
							uint16_t is_active = mb_mapping->tab_registers[base_conf + 5];
							if (is_active == 1) {
								uint16_t ip_high = mb_mapping->tab_registers[base_conf + 0];
								uint16_t ip_low = mb_mapping->tab_registers[base_conf + 1];
								uint16_t port = mb_mapping->tab_registers[base_conf + 2];
								uint16_t slave_id = mb_mapping->tab_registers[base_conf + 3];
								char ip_str[32];

								sprintf(ip_str, "%d.%d.%d.%d", (ip_high
									>> 8) & 0xFF, ip_high & 0xFF, (ip_low >> 8) &
									0xFF, ip_low & 0xFF);

								modbus_t* ctx_dev = modbus_new_tcp(ip_str, port);
								if (ctx_dev) {
									modbus_set_slave(ctx_dev, slave_id);
									modbus_set_response_timeout(ctx_dev, 0, 500000);
									if (modbus_connect(ctx_dev) != -1) {
										if (modbus_write_register(ctx_dev, remote_addr, value) == -1) TRIGGER_ERROR(7);
										modbus_close(ctx_dev);
									}
									else TRIGGER_ERROR(7);
									modbus_free(ctx_dev);
								}
								else TRIGGER_ERROR(7);
							}
							else TRIGGER_ERROR(7);
						}
						else TRIGGER_ERROR(7);
					}

					else if (opcode == 100) { // ABS (Int) 
						int16_t val = mb_mapping->tab_registers[src1];
						mb_mapping->tab_registers[dest] = (uint16_t)(val < 0 ? -val : val);
					}
					else if (opcode == 101) { // FABS (Real) 
						float fval;

						uint32_t c1 =
							((uint32_t)mb_mapping->tab_registers[src1 + 1] << 16) |
							mb_mapping->tab_registers[src1];
						std::memcpy(&fval, &c1, 4);
						fval = std::abs(fval);
						uint32_t c_res; std::memcpy(&c_res, &fval, 4);
						mb_mapping->tab_registers[dest] = c_res & 0xFFFF;
						mb_mapping->tab_registers[dest + 1] = (c_res >> 16) & 0xFFFF;
					}
					else if (opcode >= 102 && opcode <= 105) { // FSIN, FCOS, FTAN, FSQRT 
						float fval;

						uint32_t c1 =
							((uint32_t)mb_mapping->tab_registers[src1 + 1] << 16) |
							mb_mapping->tab_registers[src1];
						std::memcpy(&fval, &c1, 4);

						float fres = 0.0f;
						if (opcode == 102) fres = std::sin(fval);
						else if (opcode == 103) fres = std::cos(fval);
						else if (opcode == 104) fres = std::tan(fval);
						else if (opcode == 105) {
							if (fval < 0.0f) TRIGGER_ERROR(5)
							else fres = std::sqrt(fval);
						}

						uint32_t c_res; std::memcpy(&c_res, &fres, 4);
						mb_mapping->tab_registers[dest] = c_res & 0xFFFF;
						mb_mapping->tab_registers[dest + 1] = (c_res >> 16) & 0xFFFF;
					}
					else if (opcode == 106) { // STRLEN 
						uint16_t base = src1;
						int len = 0;
						while (base + (len / 2) < 10000) {
							uint16_t reg_val = mb_mapping->tab_registers[base + (len / 2)];
							char c = (len % 2 == 0) ? (reg_val >> 8) : (reg_val & 0xFF);
							if (c == '\0') break;
							len++;
							if (len > 2000) break; // Güvenlik sınırı 
						}
						mb_mapping->tab_registers[dest] = len;
					}

					// --- POINTERS & ARRAYS --- 
					else if (opcode == 30) {

						uint16_t base =
							mb_mapping->tab_registers[src1]; uint16_t offset =
							mb_mapping->tab_registers[src2];

						if (base + offset < 10000)
							mb_mapping->tab_registers[dest] = mb_mapping->tab_registers[base +
							offset]; else TRIGGER_ERROR(3);
					}
					else if (opcode == 31) {

						uint16_t base =
							mb_mapping->tab_registers[dest]; uint16_t offset =
							mb_mapping->tab_registers[src2];

						if (base + offset < 10000)
							mb_mapping->tab_registers[base + offset] =
							mb_mapping->tab_registers[src1]; else TRIGGER_ERROR(3);
					}
					else if (opcode == 32) {

						uint16_t base =
							mb_mapping->tab_registers[src1]; uint16_t offset =
							mb_mapping->tab_registers[src2] * 2;

						if (base + offset + 1 < 10000) {
							mb_mapping->tab_registers[dest] = mb_mapping->tab_registers[base +
								offset]; mb_mapping->tab_registers[dest + 1] =
								mb_mapping->tab_registers[base + offset + 1];
						}
						else TRIGGER_ERROR(3);
					}
					else if (opcode == 33) {

						uint16_t base =
							mb_mapping->tab_registers[dest]; uint16_t offset =
							mb_mapping->tab_registers[src2] * 2;

						if (base + offset + 1 < 10000) {
							mb_mapping->tab_registers[base + offset] =
								mb_mapping->tab_registers[src1]; mb_mapping->tab_registers[base +
								offset + 1] = mb_mapping->tab_registers[src1 + 1];
						}
						else TRIGGER_ERROR(3);
					}

					// --- METİN BİRİMİ (POINTER) --- 
					else if (opcode == 40) {

						uint16_t base =
							mb_mapping->tab_registers[src1]; uint16_t char_idx =
							mb_mapping->tab_registers[src2];
						uint16_t reg_idx = char_idx / 2; bool is_low = (char_idx % 2 != 0);
						if (base + reg_idx < 10000) {
							uint16_t reg_val = mb_mapping->tab_registers[base + reg_idx];
							mb_mapping->tab_registers[dest] = is_low ? (reg_val & 0xFF) : (reg_val >> 8);
						}
						else TRIGGER_ERROR(3);
					}
					else if (opcode == 41) {

						uint16_t char_val =
							mb_mapping->tab_registers[src1] & 0xFF; uint16_t char_idx =
							mb_mapping->tab_registers[src2];

						uint16_t base =
							mb_mapping->tab_registers[dest]; uint16_t reg_idx = char_idx / 2;
						bool is_low = (char_idx % 2 != 0);
						if (base + reg_idx < 10000) {
							uint16_t reg_val = mb_mapping->tab_registers[base + reg_idx];

							mb_mapping->tab_registers[base +
								reg_idx] = is_low ? ((reg_val & 0xFF00) | char_val) : ((reg_val
									& 0x00FF) | (char_val << 8));
						}
						else TRIGGER_ERROR(3);
					}

					// --- TIP DÖNÜŞTÜRÜCÜ --- 
					else if (opcode == 60) {
						int16_t val = mb_mapping->tab_registers[src1]; float fval = (float)val;
						uint32_t c_res; std::memcpy(&c_res, &fval, 4);

						mb_mapping->tab_registers[dest] = c_res
							& 0xFFFF; mb_mapping->tab_registers[dest + 1] = (c_res >>
								16) & 0xFFFF;
					}
					else if (opcode == 61) {

						float fval; uint32_t c1 =
							((uint32_t)mb_mapping->tab_registers[src1 + 1] << 16) |
							mb_mapping->tab_registers[src1];
						std::memcpy(&fval, &c1, 4); mb_mapping->tab_registers[dest] = (uint16_t)(int16_t)fval;
					}

					// --- FLOAT / REAL İŞLEMCİ --- 
					else if (opcode >= 50) {
						float fval1 = 0.0f, fval2 = 0.0f, fresult = 0.0f;

						uint32_t c1 =
							((uint32_t)mb_mapping->tab_registers[src1 + 1] << 16) |
							mb_mapping->tab_registers[src1];
						std::memcpy(&fval1, &c1, 4);

						if (opcode < 55 || opcode >= 62) {

							uint32_t c2 =
								((uint32_t)mb_mapping->tab_registers[src2 + 1] << 16) |
								mb_mapping->tab_registers[src2];
							std::memcpy(&fval2, &c2, 4);
						}

						if (opcode >= 62) {
							int16_t res = 0;
							if (opcode == 62) res = (fval1 == fval2) ? 1 : 0;
							else if (opcode == 63) res = (fval1 != fval2) ? 1 : 0;
							else if (opcode == 64) res = (fval1 > fval2) ? 1 : 0;
							else if (opcode == 65) res = (fval1 < fval2) ? 1 : 0;
							else if (opcode == 66) res = (fval1 >= fval2) ? 1 : 0;
							else if (opcode == 67) res = (fval1 <= fval2) ? 1 : 0;
							mb_mapping->tab_registers[dest] = res;
						}
						else {
							if (opcode == 51) fresult = fval1 + fval2;
							else if (opcode == 52) fresult = fval1 - fval2;
							else if (opcode == 53) fresult = fval1 * fval2;
							else if (opcode == 54) {
								if (fval2 == 0.0f) TRIGGER_ERROR(2)
								else fresult = fval1 / fval2;
							}
							else if (opcode == 55) fresult = fval1;
							else if (opcode == 56) fresult = fval1 + 1.0f;
							else if (opcode == 57) fresult = fval1 - 1.0f;

							uint32_t c_res; std::memcpy(&c_res, &fresult, 4);
							mb_mapping->tab_registers[dest] = c_res & 0xFFFF;
							mb_mapping->tab_registers[dest + 1] = (c_res >> 16) & 0xFFFF;
						}
					}
					// --- STANDART INT İŞLEMCİ --- 
					else {
						int16_t val1 = mb_mapping->tab_registers[src1];
						int16_t val2 = mb_mapping->tab_registers[src2];
						int16_t result = 0;

						if (opcode == 1) result = val1 + val2;
						else if (opcode == 2) result = val1 - val2;
						else if (opcode == 3) result = val1 * val2;
						else if (opcode == 4) { if (val2 == 0) TRIGGER_ERROR(1) else result = val1 / val2; }
						else if (opcode == 5) { if (val2 == 0) TRIGGER_ERROR(1) else result = val1 % val2; }
						else if (opcode == 6) result = val1 & val2;
						else if (opcode == 7) result = val1 | val2;
						else if (opcode == 8) result = val1 ^ val2;
						else if (opcode == 9) result = ~(val1 & val2);
						else if (opcode == 10) result = ~(val1 | val2);
						else if (opcode == 11) result = ~(val1 ^ val2);
						else if (opcode == 12) result = val1 << val2;
						else if (opcode == 13) result = val1 >> val2;
						else if (opcode == 14) result = (val1 << val2) | (val1 >> (16 - val2));
						else if (opcode == 15) result = (val1 >> val2) | (val1 << (16 - val2));
						else if (opcode == 16) result = (val1 == val2) ? 1 : 0;
						else if (opcode == 17) result = (val1 != val2) ? 1 : 0;
						else if (opcode == 18) result = (val1 > val2) ? 1 : 0;
						else if (opcode == 19) result = (val1 < val2) ? 1 : 0;
						else if (opcode == 20) result = (val1 >= val2) ? 1 : 0;
						else if (opcode == 21) result = (val1 <= val2) ? 1 : 0;
						else if (opcode == 22) result = val1;
						else if (opcode == 23) result = val1 + 1;
						else if (opcode == 24) result = val1 - 1;
						else if (opcode == 25) result = ~val1;
						else if (opcode == 26) result = -val1;

						mb_mapping->tab_registers[dest] = (uint16_t)result;
					}

					ips[t_idx]++;
					exec_limit--;
					chunk_steps--;
				}
				else {
					error_flag = true;
					break;
				}
			}

			if (error_flag || exec_limit <= 0) {
				mb_mapping->tab_registers[8500] = 3;
				t_active[0] = false; t_active[1] = false;
				if (exec_limit <= 0) syslog(LOG_ERR, "MACRO WATCHDOG TETIKLENDI! KOD DURDURULDU.");
			}
		}

		// === DINAMIK MODBUS CIHAZLARINI TARAMA MOTORU === 
		now = time(NULL);

		for (int i = 0; i < MAX_DEVICES; i++) {
			int base_conf = 1001 + (i * 6);
			uint16_t is_active = mb_mapping->tab_registers[base_conf + 5];

			if (is_active != 1) continue;

			uint16_t scan_time = mb_mapping->tab_registers[base_conf + 4];
			if (scan_time < 1) scan_time = 1;

			if (now - last_scan_times[i] >= scan_time) {
				uint16_t ip_high = mb_mapping->tab_registers[base_conf + 0];
				uint16_t ip_low = mb_mapping->tab_registers[base_conf + 1];

				char ip_str[32];
				sprintf(ip_str, "%d.%d.%d.%d",
					(ip_high >> 8) & 0xFF, ip_high & 0xFF,
					(ip_low >> 8) & 0xFF, ip_low & 0xFF);

				uint16_t port = mb_mapping->tab_registers[base_conf + 2];
				uint16_t slave_id = mb_mapping->tab_registers[base_conf + 3];

				modbus_t* ctx_device = modbus_new_tcp(ip_str, port);
				if (ctx_device) {
					modbus_set_slave(ctx_device, slave_id);
					modbus_set_response_timeout(ctx_device, 0, 500000);

					if (modbus_connect(ctx_device) != -1) {
						int base_data = 9000 + (i * 200);
						uint16_t read_regs[6] = { 0 };

						if (modbus_read_registers(ctx_device, 9000, 6, read_regs) != -1) {
							std::memcpy(&mb_mapping->tab_registers[base_data], read_regs, 6 * sizeof(uint16_t));
						}

						modbus_write_registers(ctx_device, 9100, 6, &mb_mapping->tab_registers[base_data + 100]);
						modbus_close(ctx_device);
					}
					modbus_free(ctx_device);
				}
				last_scan_times[i] = now;
			}
		}
	}

	modbus_mapping_free(mb_mapping);
	close(server_socket);
	modbus_free(ctx_pc);
	closelog();
	return 0;
}