/*
 Navicat Premium Data Transfer

 Source Server         : dockerMysql
 Source Server Type    : MySQL
 Source Server Version : 80025
 Source Host           : 127.0.0.1:6031
 Source Schema         : sword_nine

 Target Server Type    : MySQL
 Target Server Version : 80025
 File Encoding         : 65001

 Date: 28/07/2021 14:07:02
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for sn_class
-- ----------------------------
DROP TABLE IF EXISTS `sn_class`;
CREATE TABLE `sn_class`  (
  `Name` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '名称',
  `Code` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '编码',
  `ClassDirectoryId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '类别目录id：外键（类别目录表）',
  `TableName` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '表名称',
  `DataSourceId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '数据源id：外键（数据源表）',
  `Version` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '并发版本控制',
  `CreateTime` datetime NOT NULL COMMENT '创建时间',
  `IsDeleted` tinyint(1) NOT NULL COMMENT '数据状态',
  `LastUpdateTime` datetime NOT NULL COMMENT '最后更新时间',
  `LastUpdateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '最后更新人',
  `CreateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '创建人',
  `SeqNo` int NOT NULL COMMENT '排序号',
  `Id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT 'Id',
  `Status` int NOT NULL COMMENT '状态，0表示工作中；1表示已发布',
  `Remark` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '备注',
  `ParentId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '父类、父类必须是抽象类',
  `Type` int NOT NULL COMMENT '类型：【0：业务类别】【1：抽象类】',
  `Path` varchar(2048) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '路径',
  `TopId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '顶级类别id',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_bin COMMENT = '类别表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sn_class_attribute
-- ----------------------------
DROP TABLE IF EXISTS `sn_class_attribute`;
CREATE TABLE `sn_class_attribute`  (
  `Name` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '名称',
  `Code` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '编码',
  `ColumnName` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '列名',
  `IsPrimary` bit(1) NOT NULL COMMENT '是否主键',
  `IsNullable` bit(1) NOT NULL COMMENT '是否非空',
  `ValueType` int NOT NULL COMMENT '值类型',
  `Length` int NOT NULL COMMENT '长度',
  `Precision` int NOT NULL COMMENT '精度',
  `DefaultValue` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '默认值',
  `Remark` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '备注',
  `Description` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '描述',
  `DataSourceId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '所属数据源id',
  `ReferenceId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '引用l类别id：',
  `ClassId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '类别id：外键（类别表）',
  `Version` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '并发版本控制',
  `CreateTime` datetime NOT NULL COMMENT '创建时间',
  `IsDeleted` tinyint(1) NOT NULL COMMENT '数据状态',
  `LastUpdateTime` datetime NOT NULL COMMENT '最后更新时间',
  `LastUpdateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '最后更新人',
  `CreateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '创建人',
  `SeqNo` int NOT NULL COMMENT '排序号',
  `Id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT 'Id',
  `Status` int NOT NULL COMMENT '状态，0表示正常；1表示禁用',
  `InheritId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL DEFAULT '' COMMENT '继承属性id：（该属性是从上级类别哪个属性继承下来的，默认为空字符串）',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_bin COMMENT = '类别属性' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sn_class_directory
-- ----------------------------
DROP TABLE IF EXISTS `sn_class_directory`;
CREATE TABLE `sn_class_directory`  (
  `Path` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '目录路径',
  `ParentId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '上级目录id',
  `Name` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '名称',
  `Code` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '编码',
  `DataSourceId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '数据源id：外键（数据源表）',
  `Version` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '并发版本控制',
  `CreateTime` datetime NOT NULL COMMENT '创建时间',
  `IsDeleted` tinyint(1) NOT NULL COMMENT '数据状态',
  `LastUpdateTime` datetime NOT NULL COMMENT '最后更新时间',
  `LastUpdateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '最后更新人',
  `CreateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '创建人',
  `SeqNo` int NOT NULL COMMENT '排序号',
  `Id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT 'Id',
  `Status` int NOT NULL COMMENT '状态，0表示正常；1表示禁用',
  `Content` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '内容',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_bin COMMENT = '类别目录' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sn_data_source
-- ----------------------------
DROP TABLE IF EXISTS `sn_data_source`;
CREATE TABLE `sn_data_source`  (
  `NameSpace` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '命名空间',
  `Type` int NOT NULL COMMENT '类型：【0：MYSQL】、【1：SQLServer】、【2：Oracle】、【3：SqlLite】',
  `Name` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '名称',
  `Code` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '编码',
  `Host` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '主机',
  `Port` int NOT NULL COMMENT '端口',
  `UserName` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '用户名',
  `Password` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '密码',
  `DatabaseName` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '数据库名称',
  `Description` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '描述',
  `Version` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '并发版本控制',
  `CreateTime` datetime NOT NULL COMMENT '创建时间',
  `IsDeleted` tinyint(1) NOT NULL COMMENT '数据状态',
  `LastUpdateTime` datetime NOT NULL COMMENT '最后更新时间',
  `LastUpdateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '最后更新人',
  `CreateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '创建人',
  `SeqNo` int NOT NULL COMMENT '排序号',
  `Id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT 'Id',
  `Status` int NOT NULL COMMENT '状态，0表示正常；1表示禁用',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_bin COMMENT = '数据源表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sn_rule
-- ----------------------------
DROP TABLE IF EXISTS `sn_rule`;
CREATE TABLE `sn_rule`  (
  `EnumValue` int NOT NULL COMMENT '枚举值',
  `ValueType` int NOT NULL COMMENT '值类型：【0：布尔】',
  `Name` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '名称',
  `Code` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '编码',
  `Description` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '描述',
  `Version` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '并发版本控制',
  `CreateTime` datetime NOT NULL COMMENT '创建时间',
  `IsDeleted` tinyint(1) NOT NULL COMMENT '数据状态',
  `LastUpdateTime` datetime NOT NULL COMMENT '最后更新时间',
  `LastUpdateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '最后更新人',
  `CreateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '创建人',
  `SeqNo` int NOT NULL COMMENT '排序号',
  `Id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT 'Id',
  `Status` int NOT NULL COMMENT '状态，0表示正常；1表示禁用',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_bin COMMENT = '规则表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sn_rule_class_attribute_setting
-- ----------------------------
DROP TABLE IF EXISTS `sn_rule_class_attribute_setting`;
CREATE TABLE `sn_rule_class_attribute_setting`  (
  `RuleId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '规则id',
  `ClassId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '类别id',
  `ClassAttributeId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '属性id',
  `Value` varchar(2048) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '值',
  `Version` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '并发版本控制',
  `CreateTime` datetime NOT NULL COMMENT '创建时间',
  `IsDeleted` tinyint(1) NOT NULL COMMENT '数据状态',
  `LastUpdateTime` datetime NOT NULL COMMENT '最后更新时间',
  `LastUpdateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '最后更新人',
  `CreateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '创建人',
  `SeqNo` int NOT NULL COMMENT '排序号',
  `Id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT 'Id',
  `Status` int NOT NULL COMMENT '状态，0表示正常；1表示禁用',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_bin COMMENT = '规则类别属性配置表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sn_template
-- ----------------------------
DROP TABLE IF EXISTS `sn_template`;
CREATE TABLE `sn_template`  (
  `Description` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '描述',
  `Type` int NOT NULL COMMENT '类型：【0：后端】、【1·：前端】',
  `Code` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '编码',
  `Name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '模板名称',
  `StartFileName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '启动文件名称',
  `AssemblyDirectory` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '程序所在目录',
  `Version` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '并发版本控制',
  `CreateTime` datetime NOT NULL COMMENT '创建时间',
  `IsDeleted` tinyint(1) NOT NULL COMMENT '数据状态',
  `LastUpdateTime` datetime NOT NULL COMMENT '最后更新时间',
  `LastUpdateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '最后更新人',
  `CreateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '创建人',
  `SeqNo` int NOT NULL COMMENT '排序号',
  `Id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT 'Id',
  `Status` int NOT NULL COMMENT '状态，0表示正常；1表示禁用',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_bin COMMENT = '模板表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for tablename
-- ----------------------------
DROP TABLE IF EXISTS `tablename`;
CREATE TABLE `tablename`  (
  `Path` varchar(2048) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `Status` int NOT NULL,
  `Name` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT '机构名称',
  `Id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `CreateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `TenantId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `ParentId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `CreateTime` datetime NOT NULL,
  `TopId` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `IsDeleted` bit(1) NOT NULL,
  `LastUpdateTime` datetime NOT NULL,
  `LastUpdateUser` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_bin ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
