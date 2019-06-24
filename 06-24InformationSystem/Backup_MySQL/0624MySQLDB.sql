CREATE DATABASE  IF NOT EXISTS `0624system` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `0624system`;
-- MySQL dump 10.13  Distrib 5.6.17, for Win64 (x86_64)
--
-- Host: 10.45.241.217    Database: 0624system
-- ------------------------------------------------------
-- Server version	5.5.46-0+deb8u1

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `bugreport`
--

DROP TABLE IF EXISTS `bugreport`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bugreport` (
  `BugID` int(11) NOT NULL AUTO_INCREMENT,
  `DateTime` varchar(60) DEFAULT NULL,
  `UserName` varchar(60) DEFAULT NULL,
  `BugDesc` longblob,
  PRIMARY KEY (`BugID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bugreport`
--

LOCK TABLES `bugreport` WRITE;
/*!40000 ALTER TABLE `bugreport` DISABLE KEYS */;
/*!40000 ALTER TABLE `bugreport` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customers`
--

DROP TABLE IF EXISTS `customers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `customers` (
  `CustomerID` int(11) NOT NULL AUTO_INCREMENT,
  `CustomerName` varchar(45) NOT NULL,
  `CustInfo` longblob,
  `CustPrices` longblob,
  `CustContactPersons` longblob,
  PRIMARY KEY (`CustomerID`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customers`
--

LOCK TABLES `customers` WRITE;
/*!40000 ALTER TABLE `customers` DISABLE KEYS */;
INSERT INTO `customers` VALUES (1,'Linköping','åäö','<html>\n<body>\n<font face=\"Palatino Linotype\">\n<h1><center>Prislista Linköping</center></h1>\n</font>\n<font face=\"Palatino Linotype\">\n<table border=\"1\" style=\"width:100%\">\n  <tr>\n    <td><b>P-område</b></td>\n    <td><b>Timpris</b></td>		\n    <td><b>Maxpris/dygn</b></td>\n    <td><b>Månadsbiljett</b></td>\n  </tr>\n  <tr>\n    <td>Baggen</td>\n    <td>15 kr</td>		\n    <td>85 kr</td>\n    <td>1300 kr</td>\n  </tr>\n  <tr>\n    <td>Druvan</td>\n    <td>14 kr</td>		\n    <td>85 kr</td>\n    <td>800 kr</td>\n  </tr>\n   <tr>\n   <td>Detektiven</td>\n   <td>15 kr</td>\n   <td>85 kr</td>\n   <td>950 kr</td>\n   </tr>\n   <tr>\n   <td>Akilles</td>\n   <td>14 kr</td>\n   <td>85 kr</td>\n   <td>800 kr</td>\n   </tr>\n   <tr>\n   <td>Tinnis</td>\n   <td>18 kr</td>\n   <td>130 kr</td>\n   <td>Finns inte</td>\n   </tr>\n   <tr>\n   <td>US Norra</td>\n   <td>15 kr</td>\n   <td>85 kr</td>\n   <td>Finns inte</td>\n</table>\n<br>\nBiljettförlust kostar 200 kr på samtliga P-hus förutom flygplatsen, där du får räkna ut priset beroende på hur länge kunden har stått.\n<br><br>\nFakturaavgiften är 200 kronor, på alla P-områden utom flygplatsen där det inte tillkommer någon fakturaavgift. Det kostar inget utöver detta, så det blir som en tappad biljett. Var noga med att informera kunden om detta ifall ni skulle ta en faktura.\n<br><br>\n<b>Månadsbiljetter i flera P-hus</b>\n<br>\nMånadsbiljett i alla hus: 1300 kr<br>\nMånadsbiljett i Druvan, Detektiven, Akilles: 950 kr<br>\nMånadsbiljett i Druvan, Akilles: 800 kr<br>\n<br>\n<b>Flygplatsen</b><br>\nPå flygplatsen räknas kostnaden lite annorlunda:<br>\nDag 1-4: 120 kr per dag<br>\nDag 5-18: 40 kr per dag<br>\nDag 18-31: 0 kr per dag<br>\n<br>\nEn vecka kostar 520 kronor.<br>\nTvå veckor kostar 1040 kronor.<br>\nMax för en månad är 1040 kronor. Står man i två månader blir det 2080 kronor och så vidare.\n</font>\n</body>\n</html>','Pehr Lundgren 013-20 54 22, 0705-343115 <br>\n<i>Ring <b>alltid</b> Pehr i första hand om han jobbar</i> <br>\n<br>\nRolf 013-20 54 23\n<br>\nFrida 013-20 54 21\n<br>\nFredrik 013-20 54 26'),(2,'Bryggmagasinen','Test','åöä','åäö'),(3,'Karlstad Flygplats','.',NULL,NULL),(4,'Karlstad Sjukhus','.',NULL,NULL),(5,'Sandnes','.',NULL,NULL),(6,'Sundsvall Kraften Invest','.',NULL,NULL),(7,'Helsingborgs Lasarett','.',NULL,NULL),(8,'Jönköping Flygplats','.',NULL,NULL),(9,'Ängelholm Flygplats','.',NULL,NULL),(11,'Fornebu','.',NULL,NULL),(12,'Vasakronan','.',NULL,NULL),(13,'Trelleborg','.',NULL,NULL),(14,'Voss','.',NULL,NULL),(15,'Forskningsparken','.',NULL,NULL),(16,'Skellefteå Brinken','.',NULL,NULL),(17,'Skellefteå Flygplats','.',NULL,NULL);
/*!40000 ALTER TABLE `customers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `equipment`
--

DROP TABLE IF EXISTS `equipment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `equipment` (
  `EquipmentID` int(11) NOT NULL AUTO_INCREMENT,
  `EquipmentName` varchar(45) NOT NULL,
  `CustomerID` int(11) NOT NULL,
  `InformationID` int(11) NOT NULL,
  `ICXString` varchar(45) DEFAULT NULL,
  `InformationBLOB` longblob NOT NULL,
  PRIMARY KEY (`EquipmentID`)
) ENGINE=InnoDB AUTO_INCREMENT=169 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `equipment`
--

LOCK TABLES `equipment` WRITE;
/*!40000 ALTER TABLE `equipment` DISABLE KEYS */;
INSERT INTO `equipment` VALUES (1,'BA111',1,1,'1012','<font face=\"Palatino Linotype\">\n<center><h1>BA 111</h1></center></font>\n<font face=\"Palatino Linotype\">Betalautomat i P-huset Baggen, där kunderna kan betala med mynt och kort. Bredvid BA 111 finns BA 115, där det endast går att betala med kort.</font>'),(2,'BA112',1,2,'1013','<center><h1>BA 112</h1></center>\nBetalautomat i P-huset Baggen, där kunderna kan betala med mynt och kort. Bredvid BA 112  finns BA 113, där det endast går att betala med kort.'),(4,'IN21',2,4,'1004','Bara kort och abonnemang, inga biljetter.'),(5,'BA113',1,5,'1014','<center><h1>BA 113</h1></center>\nBetalautomat i P-huset Baggen, där kunderna endast kan betala med kort. Bredvid BA 113 finns BA 112, där det endast går att betala med både mynt och kort.'),(6,'BA114',1,6,'1015','<center><h1>BA 114</h1></center>\nBetalautomat i P-huset Baggen, där kunderna endast kan betala med kort. BA 114 är helt fristående. Vill ni be kunden betala i en annan automat så är det lättast att hänvisa till utfarten där kunden kan försöka, dock endast med kort. Behöver kunden betala med mynt så kan ni hänvisa till betalautomaterna i trapphuset vid infarten från Klostergatan.'),(7,'BA115',1,7,'1016','<center><h1>BA 115</h1></center>\nBetalautomat i P-huset Baggen, där kunderna endast kan betala med kort. Bredvid BA 115 finns BA 111, där det endast går att betala med både mynt och kort.\n'),(8,'IN121',1,8,'1017','<center><h1>IN 121</h1></center>\nInfart i P-huset Baggen, där det går att åka in via:\n<br>\n- Pappersbiljett<br>\n- SwappAccess<br>\n- Månadsbiljett (även det pappersbiljett)<br>'),(9,'IN122',1,9,'1018','<center><h1>IN 122</h1></center>\nInfart i P-huset Baggen, där det går att åka in via:\n<br>\n- Pappersbiljett<br>\n- SwappAccess<br>\n- Månadsbiljett (även det pappersbiljett)<br>\n'),(10,'UT141',1,10,'1019','<center><h1>UT 141</h1></center>\nUtfart i P-området Baggen, där det går att åka ut via:\n<br>\n- Betald pappersbiljett<br>\n- Kortbetalning<br>\n- SwappAccess<br>\n- Månadsbiljett (även det pappersbiljett)<br>\n'),(11,'UT142',1,11,'1020','<center><h1>UT 142</h1></center>\nUtfart i P-området Baggen, där det går att åka ut via:\n<br>\n- Betald pappersbiljett<br>\n- Kortbetalning<br>\n- SwappAccess<br>\n- Månadsbiljett (även det pappersbiljett)<br>\n\n'),(12,'IN221',1,12,'1025','.'),(13,'IN223',1,13,'1027','.'),(14,'UT241',1,14,'1029','<center><h1>UT 241</h1></center>\nUtfart i P-området Druvan, där det går att åka ut via:\n<br>\n- Betald pappersbiljett<br>\n- SwappAccess<br>\n- Månadsbiljett (även det pappersbiljett)<br>\n<br>\nEtt vanligt problem är att kunden inte har betalat biljetten när kunden kommer till utfarten, då får ni hänvisa till närmaste betalautomat som ligger rakt fram och till vänster från utfarterna. Be kunden sätta på varningsblinkers och trycka på stoppknappen (den stora runda knappen längst ner) innan de går till betalautomaterna. '),(15,'UT243',1,15,'1031','<center><h1>UT 243</h1></center>\nUtfart i P-området Druvan, där det går att åka ut via:\n<br>\n- Betald pappersbiljett<br>\n- SwappAccess<br>\n- Månadsbiljett (även det pappersbiljett)<br>\n<br>\nEtt vanligt problem är att kunden inte har betalat biljetten när kunden kommer till utfarten, då får ni hänvisa till närmaste betalautomat som ligger rakt fram och till vänster från utfarterna. Be kunden sätta på varningsblinkers och trycka på stoppknappen (den stora runda knappen längst ner) innan de går till betalautomaterna. \n'),(16,'IN321',1,16,'1036','.'),(17,'IN322',1,17,'1037','.'),(18,'UT341',1,18,'1038','<center><h1>UT 341</h1></center>\nUtfart i P-området Detektiven, där det går att åka ut via:\n<br>\n- Betald pappersbiljett<br>\n- SwappAccess<br>\n- Månadsbiljett (även det pappersbiljett)<br>\n<br>\nEtt vanligt problem är att kunden inte har betalat biljetten när kunden kommer till utfarten, då får ni hänvisa till närmaste betalautomat som ligger till vänster om utfarterna, förbi infarterna. Be kunden antingen backa eller sätta på varningsblinkers och trycka på stoppknappen (den stora runda knappen längst ner) innan de går till betalautomaterna. \n'),(19,'UT342',1,19,'1039','<center><h1>UT 342</h1></center>\nUtfart i P-området Detektiven, där det går att åka ut via:\n<br>\n- Betald pappersbiljett<br>\n- SwappAccess<br>\n- Månadsbiljett (även det pappersbiljett)<br>\n<br>\nEtt vanligt problem är att kunden inte har betalat biljetten när kunden kommer till utfarten, då får ni hänvisa till närmaste betalautomat som ligger till vänster om utfarterna, förbi infarterna. Be kunden antingen backa eller sätta på varningsblinkers och trycka på stoppknappen (den stora runda knappen längst ner) innan de går till betalautomaterna. \n\n'),(20,'IN421',1,20,'1043','.'),(21,'IN422',1,21,'1044','.'),(22,'IN423',1,22,'1045','.'),(23,'UT441',1,23,'1047','<center><h1>UT 441</h1></center>\nUtfart i P-området Akilles, där det går att åka ut via:\n<br>\n- Betald pappersbiljett<br>\n- SwappAccess<br>\n- Månadsbiljett (även det pappersbiljett)<br>\n<br>\nEtt vanligt problem är att kunden inte har betalat biljetten när kunden kommer till utfarten, då får ni hänvisa till närmaste betalautomat som ligger till vänster om utfarterna. Be kunden antingen backa eller sätta på varningsblinkers och trycka på stoppknappen (den stora runda knappen längst ner) innan de går till betalautomaterna. \n\n'),(24,'UT442',1,24,'1048','<center><h1>UT 442</h1></center>\nUtfart i P-området Akilles, där det går att åka ut via:\n<br>\n- Betald pappersbiljett<br>\n- SwappAccess<br>\n- Månadsbiljett (även det pappersbiljett)<br>\n<br>\nEtt vanligt problem är att kunden inte har betalat biljetten när kunden kommer till utfarten, då får ni hänvisa till närmaste betalautomat som ligger till vänster om utfarterna. Be kunden antingen backa eller sätta på varningsblinkers och trycka på stoppknappen (den stora runda knappen längst ner) innan de går till betalautomaterna. \n\n\n'),(25,'IN521',1,25,'1050','.'),(26,'IN522',1,26,'1051','.'),(27,'UT541',1,27,'1052','.'),(28,'UT542',1,28,'1053','.'),(29,'IN621',1,29,'1056','.'),(30,'UT641',1,30,'1057','.'),(31,'IN721',1,31,'1061','.'),(32,'IN722',1,32,'1062','.'),(33,'UT741',1,33,'1063','.'),(34,'UT742',1,34,'1064','.'),(35,'BA211',1,35,'1021','.'),(36,'BA212',1,36,'1022','.'),(37,'BA213',1,37,'1023','.'),(38,'BA214',1,38,'1024','.'),(39,'BA311',1,39,'1033','.'),(40,'BA312',1,40,'1034','.'),(41,'BA313',1,41,'1035','.'),(42,'BA411',1,42,'1040','.'),(43,'BA412',1,43,'1041','.'),(44,'BA413',1,44,'1042','.'),(46,'BA611',1,46,'1054','.'),(47,'BA612',1,47,'1055','.'),(48,'BA711',1,48,'1058','.'),(49,'BA712',1,49,'1059','.'),(50,'BA713',1,50,'1060','.'),(52,'BA511',1,51,'1049','<h1><center>BA 511</center></h1>\nBetalautomat på flygplatsen i Linköping. Går <b>endast</b> att betala med kort, inga kontanter. \n<br>\n<br>\nDetta är den enda betalautomaten på flygplatsen, men det går även att betala i utfarterna dit man kan hänvisa kunderna om det är problem med betalautomaten.\n<br>\n<br>\nDet kostar inget extra för faktura/biljettförlust utan det är endast för tiden kunden har stått parkerad.'),(53,'UT41',2,53,'1005','.'),(54,'BA11',3,54,'1006','.'),(55,'IN21 (Långtid)',3,55,'1007','.'),(56,'UT41 (Långtid)',3,56,'1010','.'),(57,'IN23 (Korttid)',3,57,'1008','.'),(58,'UT43 (Korttid)',3,58,'1011','.'),(59,'TAXI',3,59,'1009','.'),(60,'BA10',9,60,'1065','.'),(61,'BA11',9,61,'1066','.'),(62,'BA12',9,62,'1067','.'),(63,'BA13',9,63,'1068','.'),(64,'IN21',9,64,'1069','.'),(65,'IN22',9,65,'1070','.'),(66,'TF41',9,66,'1071','.'),(67,'TF42',9,67,'1072','.'),(68,'UT43',9,68,'1073','.'),(69,'UT44',9,69,'1074','.'),(70,'TAXI48',9,70,'1075','.'),(71,'TAXI49',9,71,'1076','.'),(72,'BA11',16,72,'1077','.'),(73,'BA12',16,73,'1078','.'),(74,'IN21',16,74,'1079','.'),(75,'IN22',16,75,'1080','.'),(76,'UT41',16,76,'1081','.'),(77,'UT42',16,77,'1082','.'),(78,'BA11',17,78,'1083','.'),(79,'BA12',17,79,'1084','.'),(80,'IN21',17,80,'1085','.'),(81,'IN22',17,81,'1086','.'),(82,'IN23',17,82,'1087','.'),(83,'IN24',17,83,'1088','.'),(84,'IN25',17,84,'1089','.'),(85,'UT41',17,85,'1090','.'),(86,'UT42',17,86,'1091','.'),(87,'UT43',17,87,'1092','.'),(90,'BA111',5,88,'1093','.'),(91,'BA112',5,91,'1094','.'),(92,'IN121',5,92,'1095','.'),(93,'IN122',5,93,'1096','.'),(94,'UT141',5,94,'1097','.'),(95,'UT142',5,95,'1098','.'),(96,'UT143',5,96,'1099','.'),(97,'DL151',5,97,'1100','.'),(98,'DL152',5,98,'1101','.'),(99,'BA213',5,99,'1102','.'),(100,'BA214',5,100,'1103','.'),(101,'BA215',5,101,'1104','.'),(102,'BA216',5,102,'1105','.'),(103,'BA217',5,103,'1106','.'),(104,'BA218',5,104,'1107','.'),(105,'BA219',5,105,'1108','.'),(106,'IN221',5,106,'1109','.'),(107,'IN222',5,107,'1110','.'),(108,'UT241',5,108,'1111','.'),(109,'UT242',5,109,'1112','.'),(110,'PL251',5,110,'1113','.'),(111,'DL252',5,111,'1114','.'),(112,'DL253',5,112,'1115','.'),(113,'DL254',5,113,'1116','.'),(114,'PL255',5,114,'1117','.'),(115,'IN21',11,115,'1118','.'),(116,'IN22',11,116,'1119','.'),(117,'UT41',11,117,'1120','.'),(118,'UT42',11,118,'1121','.'),(119,'BA10',8,119,'1122','.'),(120,'IN21',8,120,'1123','.'),(121,'IN22',8,121,'1124','.'),(122,'TAXI',8,122,'1125','.'),(123,'UT41',8,123,'1126','.'),(124,'UT42',8,124,'1127','.'),(126,'UT44',8,126,'1128','.'),(127,'IN321',5,127,'1130','.'),(128,'BA311',5,128,'1129','.'),(129,'UT341',5,129,'1131','.'),(130,'BA10',7,130,'1132','.'),(131,'BA11',7,131,'1133','.'),(132,'IN21',7,132,'1134','.'),(133,'IN22',7,133,'1135','.'),(134,'IN23',7,134,'1136','.'),(135,'IN25',7,135,'1138','.'),(136,'UT41',7,136,'1139','.'),(137,'UT42',7,137,'1140','.'),(138,'UT43',7,138,'1141','.'),(139,'UT45',7,139,'1143','.'),(140,'IN21',6,140,'1144','.'),(141,'UT41',6,141,'1145','.'),(142,'DL52',6,142,'1146','.'),(143,'IN21',12,143,'1147','.'),(144,'UT41',12,144,'1148','.'),(145,'DL51',12,145,'1149','.'),(146,'IN121',15,146,'1150','.'),(147,'UT141',15,147,'1151','.'),(148,'PL151',15,148,'1152','.'),(149,'IN152',15,149,'1153','.'),(150,'UT153',15,150,'1154','.'),(151,'BA11',14,151,'1155','.'),(152,'BA12',14,152,'1156','.'),(153,'IN21',14,153,'1157','.'),(154,'UT41',14,154,'1158','.'),(155,'IN22',13,155,'1159','.'),(156,'UT42',13,156,'1160','.'),(157,'DL51',7,157,'1171','.'),(158,'DL52',7,158,'1172','.'),(159,'IN P60',4,159,'1161','.'),(160,'IN P5',4,160,'1162','.'),(161,'IN P6',4,161,'1163','.'),(162,'IN P7',4,162,'1164','.'),(163,'IN P8',4,163,'1165','.'),(164,'UT P60',4,164,'1166','.'),(165,'UT P5',4,165,'1167','.'),(166,'UT P6',4,166,'1168','.'),(167,'UT P7',4,167,'1169','.'),(168,'UT P8',4,168,'1170','.');
/*!40000 ALTER TABLE `equipment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `information`
--

DROP TABLE IF EXISTS `information`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `information` (
  `InformationID` int(11) NOT NULL,
  `InformationBLOB` longblob NOT NULL,
  `Comment` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`InformationID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `information`
--

LOCK TABLES `information` WRITE;
/*!40000 ALTER TABLE `information` DISABLE KEYS */;
/*!40000 ALTER TABLE `information` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `maps`
--

DROP TABLE IF EXISTS `maps`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `maps` (
  `MapID` int(11) NOT NULL AUTO_INCREMENT,
  `CustomerID` int(11) NOT NULL,
  `MapPath` longblob NOT NULL,
  `MapName` varchar(55) NOT NULL,
  PRIMARY KEY (`MapID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `maps`
--

LOCK TABLES `maps` WRITE;
/*!40000 ALTER TABLE `maps` DISABLE KEYS */;
/*!40000 ALTER TABLE `maps` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `users` (
  `UserID` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(45) NOT NULL,
  `Pwd` varchar(45) NOT NULL,
  `Level` int(11) NOT NULL,
  PRIMARY KEY (`UserID`,`Username`),
  UNIQUE KEY `Username_UNIQUE` (`Username`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'Oscar','6A204BD89F3C8348AFD5C77C717A097A',2),(2,'Nikki','528D799D245DBB841C71F9B2AA15846F',2),(3,'0624','ED6FFFEF57AFE555DC9A1E6A78CC4C56',1);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2016-01-15 12:39:30
