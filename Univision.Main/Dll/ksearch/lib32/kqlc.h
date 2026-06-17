
/**************************************************************************
 *
 *  KQL/TS --- A TERA-BYTE STORAGE MANAGEMENT SYSTEM
 *
 *  COPYRIGHT (c) 1999-2013 KONAN TECHNOLOGY, INC.
 *  All rights are reserved. No part of this work covered by the copyright
 *  hereon may be reproduced or transmitted, in any form or by any means,
 *  electronic, mechanical, photocopying, recording or otherwise, without
 *  prior written permission of the copyright holder.
 *
 *  DATE    :  2001. 6
 *  AUTHOR  :  SEUNG HYUN YANG
 *  HISTORY :
 *
 *************************************************************************/


#ifndef	__KQLC_H
#define	__KQLC_H

#include	<stdarg.h>

#define KQLC_MAJOR_VERSION 4
#define KQLC_MINOR_VERSION 8
#define KQLC_PATCH_VERSION 0

/* language & charset */

#define LC_DEFAULT     0
#define LC_KOREAN      1
#define LC_CHINESE     2
#define LC_JAPANESE    3
#define LC_ENGLISH     4
#define LC_UNIVERSAL	5
#define LC_USER0		6
#define LC_USER1        7
#define LC_USER2        8
#define LC_USER3        9
#define LC_USER4        10
#define LC_USER5        11
#define LC_USER6        12
#define LC_USER7        13
#define LC_USER8        14
#define LC_USER9        15
#define LC_DANISH		16
#define LC_DUTCH		17
#define LC_FINNISH		18
#define LC_FRENCH		19
#define LC_GERMAN		20
#define LC_ITALIAN		21
#define LC_NORWEGIAN	22
#define LC_PORTUGUESE	23
#define LC_SPANISH		24
#define LC_SWEDISH		25
#define LC_RUSSIAN		26
#define LC_ARABIC		27
#define LC_TURKISH		28

#define CS_DEFAULT		0
#define CS_EUCKR		1
#define CS_EUCCN		2
#define CS_EUCJP		3
#define CS_UTF8			4
#define CS_USASCII		5
#define CS_BIG5			6
#define CS_SJIS			7
#define CS_USER0		8
#define CS_USER1		9
#define CS_USER2		10
#define CS_USER3		11
#define CS_USER4		12
#define CS_USER5		13
#define CS_USER6		14
#define CS_USER7		15
#define CS_USER8		16
#define CS_USER9		17
#define CS_LATIN1		18
#define CS_LATIN2		19
#define CS_LATIN3		20
#define CS_LATIN4		21
#define CS_LATIN5		22
#define CS_LATIN6		23
#define CS_LATIN7		24
#define CS_LATIN8		25
#define CS_LATIN9		26
#define CS_CYRILLIC		27
#define CS_ARABIC		28
#define CS_GREEK		29
#define CS_HEBREW		30
#define CS_THAI			31
#define CS_ENGLISH		CS_USASCII
#define CS_DANISH		CS_LATIN1
#define CS_DUTCH		CS_LATIN1
#define CS_FINNISH		CS_LATIN1
#define CS_FRENCH		CS_LATIN1
#define CS_GERMAN		CS_LATIN1
#define CS_ITALIAN		CS_LATIN1
#define CS_NORWEGIAN	CS_LATIN1
#define CS_PORTUGUESE	CS_LATIN1
#define CS_SPANISH		CS_LATIN1
#define CS_SWEDISH		CS_LATIN1
#define CS_RUSSIAN		CS_CYRILLIC
#define CS_TURKISH		CS_LATIN5
#define CS_NORDIC		CS_LATIN6


#define DF_PLAIN		0
#define DF_HTML			1

/* limit */

#define	MAX_VOLUME_NAME_SIZE	31
#define	MAX_MESSAGE_BUF_SIZE	128

/* open mode flags */

#define F_READ     	0x00000001
#define F_WRITE    	0x00000002

/* resource handle */

typedef	void* H_LINK;
typedef	void* H_VOLUME;
typedef	void* H_SESSION;
typedef	void* H_TABLE;

/* flags */
#define KQLC_USE_PAGE_RANKING_OFF					1
#define KQLC_USE_PAGE_RANKING_ON					2
#define KQLC_USE_PAGE_RANKING_ONLY					3
#define KQLC_SORTING_OFF                			4
#define KQLC_SORTING_ON                 			5
#define KQLC_ABSOLUTE_SCORE							6
#define KQLC_USE_NOUN_KEYWORD_ONLY					7
#define KQLC_USE_KEYWORD_CONTROL_ON					8
#define KQLC_USE_KEYWORD_CONTROL_OFF				9
#define KQLC_MORPH_SYNONYM_SEARCH					10
#define KQLC_USE_KEYWORD_PROX_CONTROL				11
#define KQLC_USE_KEYWORD_PROX_CONTROL_ON			KQLC_USE_KEYWORD_PROX_CONTROL
#define KQLC_USE_KEYWORD_PROX_CONTROL_OFF			12
#define KQLC_CONTROL_TERM_FREQUENCY_LEVEL_0			21
#define KQLC_CONTROL_TERM_FREQUENCY_LEVEL_1			22
#define KQLC_CONTROL_TERM_FREQUENCY_LEVEL_2			23
#define KQLC_CONTROL_TERM_FREQUENCY_LEVEL_3			24
#define KQLC_USE_KEYWORD_DENSITY_WEIGHT_ON			25
#define KQLC_USE_KEYWORD_DENSITY_WEIGHT_OFF			26
#define KQLC_TRANSITIVE_SYNONYM_SEARCH				27
#define KQLC_USE_REPETEND_SENSITIVE_SEARCH_ON		30
#define KQLC_USE_REPETEND_SENSITIVE_SEARCH_OFF		31
#define KQLC_MATCHFIELD_AGGREGATE_WEIGHT_ON         32
#define KQLC_MATCHFIELD_AGGREGATE_WEIGHT_OFF        33


#define KQLC_SEARCH_CONDITION_CLEAR_ALL							0
#define KQLC_SEARCH_CONDITION_USE_DAEMON_SYNONYM_MODULE			1
#define KQLC_SEARCH_CONDITION_GROUP_BY_REPRESENTATIVE_RECORD	2
/*
#define KQLC_SEARCH_CONDITION_USE_DAEMON_REPLICATE_MODULE		3
#define KQLC_SEARCH_CONDITION_USE_DAEMON_SIMILAR_MODULE			4
#define KQLC_SEARCH_CONDITION_REPLICATE2_DOMAIN_NO  		5
*/
#define KQLC_SEARCH_CONDITION_MATCHFIELD_AGGREGATE_WEIGHT   	9
#define KQLC_SEARCH_CONDITION_SYNONYM_SENSITIVE_RELEVANCE		10
#define KQLC_SEARCH_CONDITION_USE_KEYWORD_DENSITY_WEIGHT		11
#define KQLC_SEARCH_CONDITION_USE_LONGEST_SYNONYM_EXPANSION 	12
#define KQLC_SEARCH_CONDITION_USE_ZERO_PROXIMITY_WEIGHT			13
#define KQLC_SEARCH_CONDITION_USE_KEYWORD_SEQUENCE_WEIGHT		14
#define KQLC_SEARCH_CONDITION_CATEGORY_RANKING_IGNORE_FIELD_SEQUENCE		15
#define KQLC_SEARCH_CONDITION_ALLOW_STOP_WORD					16
#define KQLC_SEARCH_CONDITION_MORPH_SYNONYM_SEARCH				17
#define KQLC_SEARCH_CONDITION_USE_NATURAL_RELEVANCE_FOR_BOOLEAN 18


#define KQLC_QUERY_RAW								0
#define KQLC_QUERY_COOKED							1

#define KQLC_SELECT_FORWARD							0
#define KQLC_SELECT_BACKWARD						1

#define KQLC_METHOD_NATURAL							0
#define KQLC_METHOD_ANYWORD							1
#define KQLC_METHOD_SOMEWORD						2
#define KQLC_METHOD_ALLWORD							3
#define KQLC_METHOD_ALLORDER						4
#define KQLC_METHOD_ALLIN25							5
#define KQLC_METHOD_ALLORDERIN25					6
#define KQLC_METHOD_ALLADJACENT						7
#define KQLC_METHOD_ALLORDERADJACENT				8
#define KQLC_METHOD_EXPRESSION						9
#define KQLC_METHOD_SELECT							10
#define KQLC_METHOD_SIMILAR							11
#define KQLC_METHOD_ALLWORDTHRUINDEX				12
#define KQLC_METHOD_SOMEWORDTHRUINDEX				13
#define KQLC_METHOD_NATURALTHRUINDEX				14
#define KQLC_METHOD_ALLWORDNORANK					15
#define KQLC_METHOD_ANYWORDNORANK					16
#define KQLC_METHOD_EXACT							19

#define KQLC_METHOD_REPLICATE						28
#define KQLC_METHOD_SIMILAR2						29

#define KQLC_METHOD_NATURAL1            			30
#define KQLC_METHOD_NATURAL2            			31
#define KQLC_METHOD_NATURAL3            			32
#define KQLC_METHOD_NATURAL21           			33
#define KQLC_METHOD_NATURAL32           			34
#define KQLC_METHOD_NATURAL321          			35

#define KQLC_METHOD_ALLWORDSYN						36
#define KQLC_METHOD_ALLWORDTHRUINDEXSYN				37
#define KQLC_METHOD_REPLICATE2						52

#define KQLC_OPTION_COMPOUND_LEVEL_1     			0x0010
#define KQLC_OPTION_COMPOUND_LEVEL_2     			0x0020
#define KQLC_OPTION_COMPOUND_LEVEL_3     			0x0040
#define KQLC_OPTION_COMPOUND_LEVEL_4     			0x0080
#define KQLC_OPTION_COMPOUND_LEVEL_5     			0x0100

#define KQLC_CONTROL_RELEVANT_KEYWORD_COVERING 		0
#define KQLC_CONTROL_RELEVANT_KEYWORD_PROXIMITY 	1
#define KQLC_CONTROL_RELEVANT_KEYWORD_DENSITY		2

/* API */

#ifdef	__cplusplus
extern	"C" {
#endif

__declspec(dllimport)int	KQLC_Initialize(char pchMessageBuf[], char pchLabel[], char pchLinkName[], H_LINK* phl);


__declspec(dllimport)int	KQLC_Finalize(char pchMessageBuf[], H_LINK hl);

__declspec(dllimport)int	KQLC_Ping(char pchMessageBuf[], char pchHostAddress[], int nPortNo, int nWaitTimeMSEC);


__declspec(dllimport)void KQLC_SetThreadName(char name[]);

__declspec(dllimport)int  KQLC_Query(char pchMessageBuf[], H_LINK hl, const char pchOutDev[], char pchVolumeName[], char pchQueryChunk[], void (*pfCallback)(char[],char[],char[],int,int*,void*), void* pCallbackArg);

__declspec(dllimport)int	KQLC_GetLinkInfo(char pchMessageBuf[], H_LINK hl, int* pnLinkID, char pchHost[], int* pnPort);

__declspec(dllimport)int	KQLC_ConfigureRLimit(char msg[], H_LINK hl, char item[], int value);

__declspec(dllimport)void	KQLC_PutLog(H_LINK hl, char pchLogMessage[], ...);

__declspec(dllimport)void	KQLC_PutLogWithTime(H_LINK hl, char pchLogMessage[], ...);


/* session control API */

__declspec(dllimport)int	KQLC_Connect(char pchMessageBuf[], H_LINK hl, H_SESSION* p_hs);

__declspec(dllimport)void KQLC_BindMessage(H_SESSION hs, char pchMessageBuf[]);

__declspec(dllimport)int	KQLC_Disconnect(H_SESSION hs);


/* volume control API */

__declspec(dllimport)int	KQLC_OpenVolume(H_SESSION hs, char pchVolumeName[], int nFlag, H_VOLUME* phv);

__declspec(dllimport)int	KQLC_CloseVolume(H_VOLUME hv);

__declspec(dllimport)int	KQLC_ListTables(H_VOLUME hv, char*** pppchOutTableName, int* pnOutTableCount);

/* table control API */

__declspec(dllimport)int	KQLC_OpenTable(H_VOLUME hv, char pchTableName[], H_TABLE* pht);

__declspec(dllimport)int	KQLC_CloseTable(H_TABLE ht);

__declspec(dllimport)int	KQLC_DescribeTable(H_TABLE ht, int* pnRecordCount, int* pnRowidHWM, int*pnColumn, char* ppchColumnName[], char* ppchColumnDecl[], int* pnField, char* ppchFieldName[], char* ppchFieldDecl[], char* ppchFieldSize[], int* pnConstraint, char* ppchConstraintName[], char* ppchConstraintDecl[], int* pnIndex, char* ppchIndexName[], char* ppchIndexDecl[], int* pnGateway, char* ppchGatewayName[], char* ppchGatewayDecl[]);

__declspec(dllimport)int	KQLC_BindField(H_TABLE ht, char pchFieldList[]);


__declspec(dllimport)int	KQLC_Insert(H_TABLE ht, char* ppchFieldData[], int pnFieldSize[], int nLanguage, int nCharset, int nOption);

__declspec(dllimport)int	KQLC_UpdateByKey(H_TABLE ht, char* ppchKey[], int nKey, char* ppchFieldData[], int pnFieldSize[], int nLanguage, int nCharset, int nOption);

__declspec(dllimport)int	KQLC_UpdateByID(H_TABLE ht, int pnRowID[], int nCount, char* ppchFieldData[], int pnFieldSize[], int nLanguage, int nCharset, int nOption);

__declspec(dllimport)int	KQLC_UpdateSelection(H_TABLE ht, char pchExpr[], char* ppchFieldData[], int pnFieldSize[], int nLanguage, int nCharset, int nOption, int* pnOutRecordCount);

__declspec(dllimport)int	KQLC_DeleteByKey(H_TABLE ht, char* ppchKey[], int nKey, int nOption);

__declspec(dllimport)int	KQLC_DeleteByID(H_TABLE ht, int pnROWID[], int nCount, int nOption);

__declspec(dllimport)int	KQLC_DeleteSelection(H_TABLE ht, char pchExpr[], int nLanguage, int nCharset, int nOption, int* pnOutRecordCount);

__declspec(dllimport)int 	KQLC_Get(H_TABLE ht, char* pchPKey[], int nKey, int nOption, char* ppchFieldData[], int pnFieldSize[], int *pnRowID);

__declspec(dllimport)int  KQLC_GetByID(H_TABLE ht, int nRowID, int nOption, char* ppchFieldData[], int pnFieldSize[]);


__declspec(dllimport)int 	KQLC_AgeTable(H_TABLE ht);


__declspec(dllimport)int 	KQLC_AddSearchQuery(H_TABLE ht, char pchIndex[], char pchQuery[], char pchNegQuery[], int nMethod, int nFlag1, int nFlag2, int nLanguage, int nCharset);

__declspec(dllimport)int 	KQLC_AddSearchQuery2(H_TABLE ht, char pchIndex[], char pchQuery[], char pchNegQuery[], int nMethod, int nTermInterval, int nFlag1, int nFlag2, int nLanguage, int nCharset);

__declspec(dllimport)int 	KQLC_AddSearchQuery3(H_TABLE ht, char pchIndex[], char pchQuery[], char pchNegQuery[], int nMethod, int nTermInterval, int nFlag1, int nFlag2, int nAndOrFlag, int nLanguage, int nCharset);

__declspec(dllimport)int 	KQLC_AddSelectQuery(H_TABLE ht, char pchQuery[], char pchNegQuery[]);

__declspec(dllimport)int 	KQLC_AddSelectQuery2(H_TABLE ht, char pchQuery[], char pchNegQuery[], int nAndOrFlag);

__declspec(dllimport)int 	KQLC_AddSearchQueryByDocID(H_TABLE ht, char pchIndex[], char pchFieldName[], int nRowID, int nMethod, int nLanguage, int nCharset);

__declspec(dllimport)int 	KQLC_AddSearchQueryByDocValue(H_TABLE ht, char pchIndex[], char pchFieldName[], char pchDocValue[], int nDocValueSize, int nLanguage, int nCharset);

__declspec(dllimport)int 	KQLC_AddSearchQueryByDocValue2(H_TABLE ht, char pchIndex[], char pchFieldName[], char pchDocValue[], int nDocValueSize, char pchTitleValue[], int nTitleValueSize, int nFlag, int nLanguage, int nCharset);

__declspec(dllimport)int  KQLC_Search(H_TABLE ht, int pnRowID[], unsigned int pnScore[], int nStartOffset, int nCount, int* pnFilled, int *pnTotalResultCount, char pchCooked[], int *pnCookedLen, int nMaxCookedLen);

__declspec(dllimport)int  KQLC_Search2(H_TABLE ht, int pnRowID[], unsigned int pnScore[], int pnMatchFieldIndex[], int nStartOffset, int nCount, int *pnFilled, int *pnTotalResultCount, int* pnMatchFieldCount, int pnMatchFieldResultCount[], char pchCooked[], int *pnCookedLen, int nMaxCookedLen);

__declspec(dllimport)int  KQLC_QXSearch(H_TABLE ht, int pnRowID[], unsigned int pnScore[], int pQueryIndex[], int *pnFilled, int *pnTotalResultCount, int nStartOffset, int nCount, int nMaxQXCount, char *ppchCooked[], int pnCookedLen[], int *pnFilledQXCount, int nMaxCookedLen);

__declspec(dllimport)int  KQLC_QXSearch2(H_TABLE ht, int pnRowID[], unsigned int pnScore[], int pnMatchFieldIndex[], int pQueryIndex[], int *pnFilled, int *pnTotalResultCount, int* pnMatchFieldCount, int pnMatchFieldResultCount[], int nStartOffset, int nCount, int nMaxQXCount, char *ppchCooked[], int pnCookedLen[], int *pnFilledQXCount, int nMaxCookedLen);



__declspec(dllimport)int  KQLC_Retrieve(H_TABLE ht, int pnRowID[], unsigned int pnScore[], int *pnFilled, int *pnTotalResultCount, int nStartOffset, int nCount, char pchWhereClause[], char pchSortingClause[], int nLanguage, int nCharset);

__declspec(dllimport)int  KQLC_QXRetrieve(H_TABLE ht, int pnRowID[], unsigned int pnScore[], int pQueryIndex[], int *pnFilled, int *pnTotalResultCount, int nStartOffset, int nCount, int nMaxQXCount, char *ppchCooked[], int pnCookedLen[], int *pnFilledQXCount, int nMaxCookedLen, char pchWhereClause[], char pchSortingClause[], int nLanguage, int nCharset);

__declspec(dllimport)int KQLC_RetrieveXP(H_TABLE ht, int pnRowID[], int pQueryIndex[], unsigned int pnScore[], unsigned int pnAuxScore[], unsigned int pnUserKeyIdx[], int pnMatchFieldIndex[], int* pnMatchFieldCount, int pnMatchFieldResultCount[], int ppnSearchedFieldNo[][4], int *pnFilled, int *pnTotalResultCount, int nStartOffset, int nCount, int nMaxQXCount, char *ppchCooked[], int pnCookedLen[], int *pnFilledQXCount, int nMaxCookedLen, char pchWhereClause[], char pchSortingClause[], int nOptTermInterval, unsigned int nOptReserved, unsigned int nOptReserved2, int nLanguage, int nCharset);

__declspec(dllimport)int  KQLC_Select(H_TABLE ht, int pnRowID[], int *pnFilled, int *pnTotalResultCount, int nStartOffset, int nCount, char pchWhereClause[], char pchSortingClause[], int nLanguage, int nCharset);

__declspec(dllimport)int 	KQLC_SelectEntry(H_TABLE ht, int nWantedEntry, int nWantedDocID, int nKeywordBufSize, char** ppEntryKeyword, int** ppnRowID, int* pnResultEntryCount, int *pnResultDocIDCount, int nDirectionFlag);


__declspec(dllimport)int 	KQLC_GetLastROWID(H_TABLE ht, int *pnRowID);

__declspec(dllimport)int 	KQLC_GetRecordCount(H_TABLE ht, int *pnRecord);

__declspec(dllimport)int 	KQLC_ReadMetaField(H_TABLE ht, char pchField[], int nStartROWID, int nCount, void* Buf, int BufSize, int* pnOutBufLen, int* pnElementSize);

__declspec(dllimport)int 	KQLC_ReadMetaField2(H_TABLE ht, char pchField[], int nStartROWID, int nCount, void* Buf, int BufSize, int* pnOutBufLen, int* pnElementSize, int nMaxElemSize);

__declspec(dllimport)int	KQLC_PrintMetaValue(H_TABLE ht, char pchField[], void* DataBuf, char pchOutBuf[]);
__declspec(dllimport)int 	KQLC_GetROWID(H_TABLE ht, char *pchPKey[], int nKey, int *pnRowID);

__declspec(dllimport)int  KQLC_GetDocumentFrequency(H_TABLE ht, char *pchFieldName, char *pchKeyVal, int *pnPostCount, int *pnPostCount2);

__declspec(dllimport)int 	KQLC_LoadPageRankField(H_TABLE ht, char pchField[]);

__declspec(dllimport)int 	KQLC_LoadMetaField(H_TABLE ht, char pchField[]);

__declspec(dllimport)int 	KQLC_SetSearchCondition(H_TABLE ht, int flag);

__declspec(dllimport)int 	KQLC_SetSearchCondition2(H_TABLE ht, int nOpt, int nVal);

__declspec(dllimport)int 	KQLC_CookUserQuery(H_SESSION hs, char pchCooked[], int *pnCookedLen, char pchQuery[], int nCookedBufSize, int nLanguage, int nCharset, int nCompoundLevel);

__declspec(dllimport)int 	KQLC_CookUserQuery2(H_SESSION hs, char pchCooked[], int *pnCookedLen, char pchQuery[], int nCookedBufSize, int nOption, int nLanguage, int nCharset, int nCompoundLevel);

__declspec(dllimport)H_LINK KQLC_GetLinkHandleOfTable(H_TABLE ht);

__declspec(dllimport)H_SESSION	KQLC_FindSessionOfTable(H_TABLE ht);

__declspec(dllimport)H_SESSION	KQLC_FindSessionOfVolume(H_VOLUME hv);


/* miscellaneous */


__declspec(dllimport)char*	KQLC_GetVersion(void);

__declspec(dllimport)int	KQLC_SummarizeText(H_SESSION hs, char pchCookedKeywordStr[], char pchOriginalText[], int nOriginalLen, char pchOpeningMark[], char pchClosingMark[], char pchOutputBuf[], int nOutputBufSize, int* pnOutBufLen);

__declspec(dllimport)int KQLC_SummarizeTextWithDescription(H_SESSION hs, char pchKeywordList[], char pchKeywordDescription[], int nDescriptionLen, char pchOriginalText[], int nOrigianlLen, int nSummarySize, char pchOpeningMark[], char pchClosingMark[], char pchOpeningMark2[], char pchClosingMark2[], char pchOutputBuf[], int nOutputBufSize, int* pnOutBufLen);

__declspec(dllimport)int	KQLC_GetTermAssociate(H_SESSION hs, char pchWordList[], int nBufSize, int *pnResult, char pchQuery[], char pchNegQuery[], int nWanted, int nLanguage, int nCharset);

__declspec(dllimport)int	KQLC_GetTermAliases(H_SESSION hs, int* pnAliasCount, char* pchAliasVal[], int pnAliasLen[], char pchBuf[], int nBufSize, int nMaxWanted, char pchQueryVal[], int nQueryLen, int nLanguage, int nCharset);

__declspec(dllimport)int  KQLC_CheckDeletion(H_TABLE ht, int* pHasDeleted, int* pnDeleted, int pnDeletedRowID[], int nCount, int pnRowID[]);

__declspec(dllimport)int KQLC_ProcessTag(char pchMessageBuf[], H_LINK hl, char* pchTagName, char* pchInput, int nInputSize, char pchOutBuf[], int nOutBufSize, int* pOutputLen, int nLanguage, int nCharset, void* pOpt);

#ifdef	__cplusplus
}
#endif

#endif /* __KQLC_H */

