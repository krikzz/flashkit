

module flasher_md
(addr, dat, ce_lo, ce_hi, oe, we_lo, we_hi, as, tim, hrst, mrst, cas, dtak, cart, mclk, fdat, txe, rxf, fwr, frd, rst, clk, led);
 
	//cart bus
	output reg[22:0]addr;
	inout [15:0]dat;
	output ce_lo, ce_hi, we_hi, as, tim, hrst, mrst, cas, dtak, mclk;
	output reg oe, we_lo;
	input cart;
	
	//usb
	inout [7:0]fdat;
	input rxf, txe;
	output reg frd, fwr;
	
	//misk
	input rst, clk;
	output led;
	
	parameter DEV_ID = 7'h01;
	
	parameter PAR_INC = 7;
	parameter PAR_SINGLE = 6;
	parameter PAR_ID_RD = 5;
	parameter PAR_MODE8 = 4;
	
	assign hrst = !rst_st ? 0 : 1'bz;
	assign mrst = !rst_st ? 0 : 1'bz;
	assign cas = 1'bz;
	assign dtak = addr[22:21] == 0 & !as ? 0 : 1'bz;
	assign as = !oe | !we_lo ? 0 : 1;//bus_act ? 0 : 1;
	
	wire w_clk = clk_dom[2];
	reg [3:0]clk_dom;
	always @(negedge clk)clk_dom <= clk_dom + 1;
	
	reg [3:0]ret;
	reg [7:0]cmd;
	reg [15:0]len;
	reg [15:0]dat_buff;
	reg rxf_st, txe_st;
	reg exec;
	reg [3:0]state;
	
	assign fdat[7:0] = !frd ? 8'hzz : cmd[PAR_ID_RD] ? {cart, DEV_ID} : cmd[PAR_MODE8] ? dat_buff[7:0] : dat_buff[15:8];
	wire bus_act = !oe | !oe_st | !we_lo | !we_lo_st ? 1 : 0;
	wire ce = bus_act ? addr[22] : 1;
	
	assign ce_lo = ce ? 1 : !cart ? addr[21] : !addr[21];
	assign ce_hi = ce;
	reg rst_st;
	
	assign led = exec;
	assign mclk = w_clk;
	assign tim = addr[22:7] == 16'hA130 & !as ? 0 : 1;
	//assign as = 
	
	
	assign we_hi = we_lo;
	assign dat[15:0] =  !we_int | !we_lo | !we_lo_st ? dat_buff[15:0] : 16'hzzzz;
	//assign mrst = rst;
	reg we_lo_st;
	reg we_int;
	reg frd_st;
	reg oe_st;
	reg [2:0]delay;
	reg [2:0]delay_val;
	reg unlock;
	
	always @(posedge w_clk)
	if(delay[2:0] != 0)delay <= delay - 1;
		else
	begin
		
		rst_st <= rst;
		rxf_st <= !rxf;
		txe_st <= !txe;
		if(frd == 0)frd <= 1;
		if(fwr == 0)fwr <= 1;
		if(oe == 0)oe <= 1;
		if(we_lo == 0)we_lo <= 1;
		if(!exec)we_int <= 1;
		if(!exec)state <= 0;
		
		if(unlock == 0 & cmd[7:0] == 8'h62 & exec)unlock <= 1;
			else
		if(unlock == 0 & exec)exec <= 0;
			else
		if(cmd[3:0] > 5 & exec)exec <= 0;
		
		
		if(!frd)dat_buff[15:0] <= cmd[PAR_MODE8] ? {8'h00, fdat[7:0]} : {dat_buff[7:0], fdat[7:0]};
		if(!oe)dat_buff[15:0] <= dat[15:0];
		we_lo_st <= we_lo;
		oe_st <= oe;
		
		delay[2:0] <= delay_val[2:0];
		
		if(!rst_st)
		begin
			exec <= 0;
			unlock <= 0;
		end
			else
		if(!exec & frd == 0)
		begin
			cmd <= fdat;
			exec <= 1;
		end
		if(!exec & frd == 1)frd <= rxf;	
		
		//set addr
		if(exec & cmd[3:0] == 0 & frd == 1)frd <= rxf;
		if(exec & cmd[3:0] == 0 & frd == 0)
		begin
			addr[22:0] <= {addr[14:0], fdat[7:0]};
			exec <= 0;
		end
		
		//set len
		if(exec & cmd[3:0] == 1 & frd == 1)frd <= rxf;
		if(exec & cmd[3:0] == 1 & frd == 0)
		begin
			len[15:0] <= {len[7:0], fdat[7:0]};
			exec <= 0;
		end
		
		//read cart
		if(exec & cmd[3:0] == 2)
		case(state)
			0:begin
				oe <= 0;
				state <= state + 1;
			end
			1:begin
				len <= len - 1;
				state <= state + 1;
			end
			2:begin
				if(fwr)fwr <= txe;
				if(!fwr)state <= cmd[PAR_MODE8] ? state + 3 : state + 1;
			end
			3:begin
				dat_buff[15:8] <= dat_buff[7:0];
				state <= state + 1;
			end
			4:begin
				if(fwr)fwr <= txe;
				if(!fwr)state <= state + 1;
			end
			5:begin
				if(cmd[PAR_INC])addr <= addr + 1;
				if(len == 0 | cmd[PAR_SINGLE])exec <= 0;
				state <= 0;
			end
			
		endcase
		
		///write
		if(exec & cmd[3:0] == 3)
		case(state)
			0:begin
				if(frd)frd <= rxf;
				if(!frd)state <= cmd[PAR_MODE8] ? state + 2 : state + 1;
			end
			1:begin
				if(frd)frd <= rxf;
				if(!frd)state <= state + 1;
			end
			2:begin
				len <= len - 1;
				we_int <= 0;
				state <= state + 1;
			end
			3:begin
				we_lo <= 0;
				state <= state + 1;
			end
			4:begin
				state <= state + 1;
				we_int <= 1;
			end
			5:begin
				if(cmd[PAR_INC])addr <= addr + 1;
				if(len == 0 | cmd[PAR_SINGLE])exec <= 0;
				state <= 0;
			end
		endcase
		
		//ry
		
		if(exec & cmd[3:0] == 4)
		case(state)
			0:begin
				oe <= 0;
				state <= state + 1;
			end
			1:begin
				oe <= 1;
				state <= state + 1;
			end
			2:begin
				if(oe == 1)oe <= 0;
				if(oe == 0 & dat == dat_buff)exec <= 0;
			end
		endcase
		
		//set delay
		if(exec & cmd[3:0] == 5 & frd == 1)frd <= rxf;
		if(exec & cmd[3:0] == 5 & frd == 0)
		begin
			delay_val[2:0] <= fdat[2:0];
			exec <= 0;
		end
		
	end
	
	
endmodule 
