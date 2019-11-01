#!/usr/bin/python
# -*- coding: utf-8 -*-

import os,sys
import xlrd
import shutil
import xls_lua_config
import time;

INF = 999999
default_encoding = 'utf-8'
if sys.getdefaultencoding() != default_encoding:
    reload(sys)
    sys.setdefaultencoding(default_encoding)

def search(base):
    pattern = '.xlsx'
    fileresult = []
    cur_list = os.listdir(base)
    for item in cur_list:
        full_path = os.path.join(base, item)
        if os.path.isdir(full_path):
            fileresult += search(full_path)
        if os.path:
            if full_path.endswith(pattern):
                fileresult.append(full_path)
    return fileresult

def get_to_way(name):
    value = xls_lua_config.lua_config_table.get(name)
    #没找到，按照默认格式来，列表样式
    if not value:
        return None
    
    #找到了，
    return int(value["way"])

def strToDate(str):
    if(str==""):
        str="0";
    str=str.strip();
    strArr=str.split('/');
    temp=["0","0","0","0","0","0"];
    for i in range(0,len(temp)):
        if(i<len(strArr)):
            temp[i]=strArr[i];
    result='';
    if(temp[0]=="0"):
        return 0;
    else:
        for i in range(0,len(temp)):
            if(i<2):
                result=result+temp[i]+'-';
            elif(i==2):
                result=result+temp[i]+' ';
            elif(i<5):
                result=result+temp[i]+':';
            else:
                result=result+temp[i];
        #print result
        timeStamp=time.mktime(time.strptime(result,'%Y-%m-%d %H:%M:%S'));
        return int(timeStamp);

def SpecialLuaStript( excelsheet, lua_filename, tablename):
    #print "excelsheet:", excelsheet,"lua_filename:", lua_filename,"tablename:",tablename
    print "start SpecialLuaStript"
    output_lua = open(lua_filename, 'w')
    
    content = "return " + "  {\n"
    fields = []
    for row in range(1, excelsheet.nrows):
        #print "row:",row
        
        # 从第一行第二列开始遍历
        hasValue = False
        fieldDefine = ["[",str(row - 2), "]","={"]
        fieldContent = [""]
        for col in range(2, excelsheet.ncols):
            #print "col:",col
            cell = excelsheet.cell(row, col).value
            # 列， 各个字段
            # 字段名字
            if row == 1:
                #print cell
                #fields.append(cell)
                continue            

            cellValue = None
            finalCellValue = ""
            #print cell

            try:
               if cell == int(cell):
                  cellValue = int(cell)
               else:
                  cellValue = float(cell)
            except:
                cellValue = 0
                
            #print cell
            finalCellValue = str(cellValue)+","
            hasValue = True
      
            fieldContent.extend(finalCellValue)
            
                        
        if hasValue:
            fieldDefine.extend(fieldContent)
            field_str = " ".join(fieldDefine)
            content = content + field_str + "},\n"


    content = content + "}"
    output_lua.write(content)
    output_lua.close()

def GenerateLuaStript( excelsheet, lua_filename, tablename):
    print "start GenerateWay"
    output_lua = open(lua_filename, 'w')
    
    tablename = tablename + "_lua_config"

    content = "local  " + tablename + " = {\n"
    fields = { }
    types =  { }
    invalidlist = []
    for row in range(excelsheet.nrows):
        # 每行来遍历, 前三行是描述
        hasValue = False

        fieldDefine = [""]
        fieldContent = [""]

        for col in range(excelsheet.ncols):
            InvalidValue = False
            cell = excelsheet.cell(row, col).value
            # 列， 各个字段
            # 字段名字
            if row == 0 and str(cell) == "":
            #策划第1行不填，被过滤
                invalidlist.append(col)
                continue

            if invalidlist.count(col) == 1:
                continue

            if row == 0:
                fields[col] = cell
                continue
            #饥荒源码中的英文单词,策划专用
            elif row == 1:
                continue
            # 标志(0客户端和服务器共用,2服务器专用,1客户端专用)
            elif row == 3:
                commonflag = 3
                try:
                    commonflag = int(cell)
                except:
                    commonflag = 0

                filterflag = True
                if commonflag == 0:
                    filterflag = False
                elif commonflag == 2:
                    filterflag = False

                if filterflag == True :
                    invalidlist.append(col)

                continue
            # 注释
            elif row == 4:
                continue
            #字段类型  
            elif row == 2:
                if cell.lower() == "int":
                    types[col] = "int"
                elif cell.lower() == "string" :
                    types[col] = "string"
                elif cell.lower().find('date') == 0:
                    types[col] = "date"
                elif cell.lower() == "long":
                    types[col] = "long"
                elif cell.lower() == "table":
                    types[col] = "table"
                elif cell.lower() == "float":
                    types[col] = "float"
                else :
                    #raise TypeError("Paramete type isnot any defined type!")  
                    print ("=======ERROR : Paramete type isnot any defined type!==========")
                    return
                continue


            cellValue = None
            finalCellValue = ""
            if types[col] == 'int':
                hasValue = True
                try:
                    cellValue = int(cell)
                except:
                    cellValue = 0
                finalCellValue = "= "+str(cellValue)+","

            if types[col] == 'float':
                hasValue = True
                try:
                    cellValue = float(cell)
                except:
                    cellValue = 0.0
                finalCellValue = "= "+str(cellValue)+","

            elif types[col] == 'string':
                hasValue = True
                try:
                    cellValue = str(cell)
                except:
                    print "................."
                    cellValue = "nil"

                if len(cellValue) == 0:
                    finalCellValue = "= nil ,"
                else:
                    finalCellValue = "= [==["+cellValue+"]==],"

            elif types[col] == 'table':
                hasValue = True
                try:
                    cellValue = str(cell)
                except:
                    print "................."
                    cellValue = "nil"

                if cellValue != "":
                     #finalCellValue = "= [==[ "  + "return { " + cellValue + " } ]==],"
                     finalCellValue = "= "  + " { " + str(cellValue) + " } ,"
                else:
                    InvalidValue = True

            elif types[col] == 'date':
                hasValue = True
                try:
                    cellValue=strToDate(str(cell))
                except:
                    cellValue = 0
                finalCellValue = "= "+str(cellValue)+","
            elif types[col] == 'long':
                hasValue = True
                try:
                    cellValue = long(cell)
                except:
                    cellValue = 0
                finalCellValue = "= "+str(cellValue)+","

            if col == 0:
                fieldDefine = ["[", str(cellValue), "]={"]

            if InvalidValue == False:
                fieldContent.extend([fields[col].lower(), finalCellValue])

        if hasValue :
            fieldDefine.extend(fieldContent)
            field_str = " ".join(fieldDefine)
            content = content + field_str + "},\n"


    content = content + "}"

    content = content + "\n"
    content = content + "\n"

    content = content + "return  " + tablename
    output_lua.write(content)
    output_lua.close()


def run(temp_excel_file_dir):

    lua_path = "lua_config/"
    for result in search(temp_excel_file_dir):
        
        excelname = os.path.basename(result).replace(".xlsx", "")
        #print excelname

        if get_to_way(excelname) == None or get_to_way(excelname) == 2:
            continue
        
        print 'start %s'%os.path.basename(result)
        a = xlrd.open_workbook(result)
        for i in a.sheets():
            lua_filename = (lua_path + excelname.decode('utf-8') + ".lua").lower()
            #print "go",i
            if os.path.exists(lua_filename):
                os.remove(lua_filename)
            #转换程序(常规转换和特殊转换)

	    value = get_to_way(excelname)
	    #print value
            if get_to_way(excelname) == 0:
               GenerateLuaStript( i, lua_filename, excelname)
            elif get_to_way(excelname) == 1:
               SpecialLuaStript( i, lua_filename, excelname)
            break

        print 'finish %s'%os.path.basename(result)


if __name__ == '__main__':
    
    print "ExcelToLua start"
    current_path = os.getcwd().replace('\\', '/')
    root_path = os.path.abspath(os.path.join(os.getcwd(), "../")).replace('\\', '/')

    excel_list = []
    for arg in sys.argv[1:]:
        excel_list.append(arg)
        print arg


    #excel_file_dir = current_path + '/excel'.decode('utf-8');
    excel_file_dir = root_path + '/excel/'.decode('utf-8');
    temp_excel_file_dir = current_path + '/temp_file/excel/'.decode('utf-8');

    #建立一个临时表，方便操作
    for result in search(excel_file_dir):
        if len(excel_list) > 0:
            if (os.path.basename(result) in excel_list) and (os.path.basename(result).find('~$') == -1):
                shutil.copyfile(result, temp_excel_file_dir + os.path.basename(result))
        elif os.path.basename(result).find('~$') == -1:
            shutil.copyfile(result, temp_excel_file_dir + os.path.basename(result))
    #主执行程序
    run(temp_excel_file_dir)
    
    
