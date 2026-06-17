/**************************************************************************
*
*  KSEARCH DAEMON --- A VERSATILE STANDARD DAEMON
*
*  COPYRIGHT (c) 1999-2013 KONAN TECHNOLOGY, INC.
*  All rights are reserved. No part of this work covered by the copyright
*  hereon may be reproduced or transmitted, in any form or by any means,
*  electronic, mechanical, photocopying, recording or otherwise, without
*  prior written permission of the copyright holder
*
*  DATE    :
*  AUTHOR  :
*  HISTORY :
*
*************************************************************************/


#ifndef __KSEARCH_H_
#define __KSEARCH_H_

#include "kqlc.h"	/* for language & charset code */

/* resource handle */
typedef void*	H_KSEARCH;

#define QEXP_K2K		0x01
#define QEXP_K2E		0x02
#define QEXP_E2E		0x04
#define QEXP_E2K		0x08
#define QEXP_TRL		0x10
#define QEXP_RCM		0x20

#define QPP_OP_NONE		0
#define QPP_OP_EQ		1
#define QPP_OP_LT		2
#define QPP_OP_LE		3
#define QPP_OP_GT		4
#define QPP_OP_GE		5

#define QPP_DOMAIN_NONE	0
#define QPP_DOMAIN_CAR	1


#define eOK				0
#define eERR			-1
#define eTIMEOUT		-2
#define eONUPDATE		-3
#define eSERVICESTOP	-4
#define eNETWORKERR		-5

#define	OPTION_SOCKET_TIMEOUT_REQUEST	1
#define	OPTION_SOCKET_TIMEOUT_LINGER	2
#define	OPTION_SOCKET_3WAY_MODE			3
#define	OPTION_SOCKET_ASYNC_REQUEST		4
#define	OPTION_SOCKET_TCP_NODELAY		5
#define	OPTION_SOCKET_CONNECTION_TIMEOUT_MSEC	6
#define	OPTION_REQUEST_PRIORITY			10
#define	OPTION_REQUEST_CHARSET_UTF8		11
#define	OPTION_RETRY_ON_NETWORK_ERROR	20
#define	OPTION_REPORT_CLIENT_ERROR		30

#define OUT
#define IN

#ifdef __cplusplus
extern	"C" {
#endif

__declspec(dllimport)int KSEARCH_CreateHandle (
				OUT char msg[], 
				OUT H_KSEARCH* phc);

__declspec(dllimport)int KSEARCH_DestroyHandle (
				OUT char msg[], 
				IN H_KSEARCH hc);

__declspec(dllimport)int KSEARCH_SetSearchOption_Cluster (
                OUT char msg[],
                IN  H_KSEARCH hc,
                IN  int nMaxCluster,
                IN  int nMaxRecordToCluster,
                IN  char pchFieldList[],
                IN  char *ppchPrevTitle[],
                IN  int nPrevTitle,	
                IN  char pchKeyList[],
                IN  int nFlag);	
	
__declspec(dllimport)int KSEARCH_SetOption (
                OUT char msg[],
                IN  H_KSEARCH hc,
                IN  int nOpt,
				IN	int nVal);

__declspec(dllimport)int KSEARCH_SetSearchOption_TimeOut (
				OUT char msg[],
				IN  H_KSEARCH hc,
				IN  int nTimeOutSec);

__declspec(dllimport)int KSEARCH_SubmitQuery (
                OUT char msg[],
                IN  H_KSEARCH hc,
                IN  char pchServerAddress[],
                IN  int nPort,
                IN  char pchAuthCode[],
                IN  char pchLog[],
                IN  char pchScn[],
                IN  char pchWhereClause[],
                IN  char pchSortingClause[],
                IN  char pchHighlightText[],
                IN  int nStartOffset,
                IN  int nRecordCount,
                IN  int nLanguage,
                IN  int nCharset);
 
__declspec(dllimport)int KSEARCH_GetResult_TotalCount (
				OUT char msg[],
				IN  H_KSEARCH hc,
				OUT int *pnTotalCount);

__declspec(dllimport)int KSEARCH_GetResult_RowSize (
                OUT char msg[],
                IN  H_KSEARCH hc,
                OUT int *pnRowSize);

__declspec(dllimport)int KSEARCH_GetResult_ColumnSize (
                OUT char msg[],
                IN  H_KSEARCH hc,
                OUT int *pnColSize);

__declspec(dllimport)int KSEARCH_GetResult_ColumnName (
                OUT char msg[],
                IN  H_KSEARCH hc,
                OUT char* ppColName[],
				IN	int nColSize);

__declspec(dllimport)int KSEARCH_GetResult_ROWID (
                OUT char msg[],
                IN  H_KSEARCH hc,
                OUT int **ppRecordNo,
                OUT unsigned int **ppScore);

__declspec(dllimport)int KSEARCH_GetResult_Row (
                OUT char msg[],
                IN  H_KSEARCH hc,
                OUT char *ppColData[],
                OUT int pColDataSize[],
                IN  int nRowNo);

__declspec(dllimport)int KSEARCH_GetResult_Cluster (
                OUT char msg[],
                IN  H_KSEARCH hc,
                OUT int *pnOutClusterCount,
                OUT char *ppOutTitleBuf[],
                OUT int *ppOutRecordNo[],
                OUT int **ppOutRecordCount);

__declspec(dllimport)int KSEARCH_GetResult_SearchTime (
                OUT char msg[],
                IN  H_KSEARCH hc,
                OUT int *pnSearchTime);

__declspec(dllimport)int KSEARCH_GetResult_Matchfield (
				OUT char msg[],
				IN	H_KSEARCH hc,
				OUT int **ppMatchfield);

__declspec(dllimport)int KSEARCH_GetResult_UserKeyIndex (
				OUT char msg[],
				IN	H_KSEARCH hc,
				OUT unsigned int **ppUserKeyIndex);

__declspec(dllimport)char* KSEARCH_GetErrorMessage (
				IN  H_KSEARCH hc);

__declspec(dllimport)int KSEARCH_SetAuthCode (
                OUT char msg[],
                IN  H_KSEARCH hc,
                IN  char pchAuthCode[]);

__declspec(dllimport)int KSEARCH_SetLog (
                OUT char msg[],
                IN  H_KSEARCH hc,
                IN  char pchLog[]);

__declspec(dllimport)int KSEARCH_Select (
                OUT char msg[],
                IN  H_KSEARCH hc,
                IN  char pchServiceAddr[],
                IN  char pchScn[],
                IN  char pchWhereClause[],
                IN  char pchSortingClause[],
                IN  char pchHighlightText[],
                IN  int  nStartOffset,
                IN  int  nRecordCount,
                IN  int  nLanguage,
                IN  int  nCharset);

__declspec(dllimport)int KSEARCH_Insert (
                OUT char msg[],
                IN  H_KSEARCH hc,
                IN  char pchServiceAddr[],
                IN  char pchFullTableName[],
                IN  char *ppchFieldName[],
                IN  char *ppchFieldData[],
                IN  int  pnFieldSize[],
                IN  int  nField,
                IN  int  nLanguage,
                IN  int  nCharset,
                IN  int  nFlag);

__declspec(dllimport)int KSEARCH_Delete (
                OUT char msg[],
                IN  H_KSEARCH hc,
                IN  char pchServiceAddr[],
                OUT int  *pnOutRecordCount,
                IN  char pchFullTableName[],
                IN  char pchExpr[],
                IN  int  nLanguage,
                IN  int  nCharset,
                IN  int  nFlag);
 
__declspec(dllimport)int KSEARCH_Update (
                OUT char msg[],
                IN  H_KSEARCH hc,
                IN  char pchServiceAddr[],
                OUT int  *pnOutRecordCount,
                IN  char pchFullTableName[],
                IN  char pchExpr[],
                IN  char *ppchFieldName[],
                IN  char *ppchFieldData[],
                IN  int  pnFieldSize[],
                IN  int  nField,
                IN  int  nLanguage,
                IN  int  nCharset,
                IN  int  nFlag);

__declspec(dllimport)int KSEARCH_SetSearchOption_ExpandQuery (
				OUT char msg[],
				IN  H_KSEARCH hc,
                IN  char pchQuery[],
                IN  int  nFlag);

__declspec(dllimport)int KSEARCH_GetResult_ExpandQuery (
				OUT char msg[],
				IN  H_KSEARCH hc,
				OUT char **ppchOutWord,
				OUT int  *pnOutWord,
                IN  int  nFlag);

__declspec(dllimport)int KSEARCH_CompleteKeyword (
				OUT char msg[],
				IN  H_KSEARCH hc,
                IN  char pchServiceAddr[],
				OUT	int  *pnKwdCount,
				OUT char *ppchKwd[],
				OUT char **ppCnvStr,
				IN	int  nMaxKwdCount,
				IN  char pchSeedStr[],
				IN	int  nFlag,
				IN	int  nDomainNo);

__declspec(dllimport)int KSEARCH_CompleteKeyword2 (
				OUT char msg[],
				IN  H_KSEARCH hc,
                IN  char pchServiceAddr[],
				OUT	int  *pnKwdCount,
				OUT char *ppchKwd[],
				OUT int	 pnRank[],
				OUT char *ppchTag[],
				OUT char *ppchNum[],
				OUT char **ppCnvStr,
				IN	int  nMaxKwdCount,
				IN  char pchSeedStr[],
				IN	int  nFlag,
				IN	int  nDomainNo);

__declspec(dllimport)int KSEARCH_SpellCheck (
				OUT char msg[],
				IN  H_KSEARCH hc,
                IN  char pchServiceAddr[],
				OUT int  *pnOutCount,
				OUT char *pchOutWord[],
				IN  int  nMaxOutCount,
				IN  char pchInWord[]);

__declspec(dllimport)int KSEARCH_RecommendKeyword (
				OUT char msg[],
				IN  H_KSEARCH hc,
                IN  char pchServiceAddr[],
				OUT	int  *pnOutCount,
				OUT char *ppchOutStr[],
				IN	int  nMaxOutCount,
				IN  char pchInStr[],
				IN	int  nDomainNo);

__declspec(dllimport)int KSEARCH_GetPopularKeyword (
				OUT char msg[],
				IN  H_KSEARCH hc,
                IN  char pchServiceAddr[],
				OUT	int  *pnOutCount,
				OUT char *ppchOutStr[],
				IN	int  nMaxOutCount,
				IN	int  nDomainNo);

__declspec(dllimport)int KSEARCH_GetPopularKeyword2 (
				OUT char msg[],
				IN  H_KSEARCH hc,
                IN  char pchServiceAddr[],
				OUT	int  *pnOutCount,
				OUT char *ppchOutStr[],
				OUT char *ppchOutTag[],
				IN	int  nMaxOutCount,
				IN	int  nDomainNo);

__declspec(dllimport)int KSEARCH_AnchorText (
				OUT char msg[],
				IN  H_KSEARCH hc,
				IN  char pchServiceAddr[],
				OUT char** ppchOutText,
				IN  char pchInText[],
				IN  char pchTagScheme[],
				IN  char pchOption[],
				IN  int nDomainNo);

__declspec(dllimport)int KSEARCH_GetSynonymList (
				OUT char msg[],
				IN  H_KSEARCH hc,
				IN  char pchServiceAddr[],
				OUT int  *pnTermCount,
				OUT int  pnSynonymCount[],
				OUT char **pppchSynonymList[],
				IN  int  nMaxTermCount,
				IN  char pchInStr[],
				IN  int  nOptPartExp,
				IN  int  nOptMorphExp,
				IN  int  nLanguage,
				IN  int  nCharset,
				IN  int  nCompoundLevel,
				IN  int  nDomainNo);

__declspec(dllimport)int KSEARCH_ExtractKeyword (
				OUT char msg[],
				IN  H_KSEARCH hc,
				IN  char pchServiceAddr[],
				OUT int* pnOutKeywordCount,
				OUT char* ppchOutKeyword[],
				IN  int nMaxOutKeywordCount,
				IN  char pchLinkName[],
				IN  char pchInputText[],
				IN  int nOption,
				IN  int nLanguage,
				IN  int nCharset,
				IN  int nCompoundLevel);


__declspec(dllimport)int KSEARCH_SetClientLogLocation (
				OUT char msg[], 
				IN  H_KSEARCH hc,
				IN  char pchPath[], 
				IN  int nOption1, 
				IN  int nOption2);

__declspec(dllimport)int KSEARCH_GetRealTimePopularKeyword (
				OUT char msg[],
				IN  H_KSEARCH hc,
                IN  char pchServiceAddr[],
				OUT	int  *pnOutCount,
				OUT char *ppchOutStr[],
				OUT char *ppchOutTag[],
				IN	int  nMaxOutCount,
				IN	int  nOptOnline,
				IN	int  nDomainNo);

__declspec(dllimport)int KSEARCH_Transliterate (
				OUT char  msg[],
				IN  H_KSEARCH  hc,
                IN  char  pchServiceAddr[],
				OUT	int*  pnTransliteratedWordCount,
				OUT char* ppchTransliteratedWord[],
				IN	int   nMaxTransliterateWordCount,
				IN  char  pchSearchWords[],
				IN  int   nTargetLanguage,
				IN  int   nDomainNo);

__declspec(dllimport)int KSEARCH_CensorSearchWords (
				OUT char  msg[],
				IN  H_KSEARCH  hc,
                IN  char  pchServiceAddr[],
				OUT	int*  pnCensoredWordCount,
				OUT char* ppchCensoredWord[],
				IN	int   nMaxCensorWordCount,
				IN  char  pchSearchWords[],
				IN  int   nDomainNo);

__declspec(dllimport)int KSEARCH_GetDocumentFrequency (
				OUT char  msg[],
				IN  H_KSEARCH  hc,
                IN  char  pchServiceAddr[],
				OUT	int*  pnPostCount,
				IN  char  pchKeyValue[],
				IN  char  pchFieldName[],
				IN  char  pchTableName[],
				IN  char  pchVolumeName[]);


__declspec(dllimport)int KSEARCH_IndexSearch (
				OUT char msg[],
				IN  H_KSEARCH hc,
				OUT int* pnRowid,
				OUT int  pnRowidList[],
				OUT char* pchOutPrimaryKeys[],
				IN  char pchAddrSearch[],
				IN  char pchVolumeTableNames[],
				IN  char pchSearchOpts[],
				IN  char pchWhereClause[],
				IN  char pchSortingClause[],
				IN  char pchRankClause[],
				IN  char pchReservedOpts1[],
				IN  char pchReservedOpts2[],
				IN  int  pnReserved3[],
				IN  int  pnReserved4[],
				IN  char pchLog[],
				IN  int  nStartOffset,
				IN  int  nWantedCount,
				IN  int  nLanguage,
				IN  int  nCharset);


__declspec(dllimport)int KSEARCH_Hilite (
				OUT char msg[],
				IN  H_KSEARCH hc,
				IN  char pchAddrDataServers[],
				IN  char pchVolumeTableNames[],
				IN  char pchFieldNames[],
				IN  int  nRowid,
				IN  int  pnRowidList[],
				IN  char* pchPrimaryKeys[],
				IN  char pchHighlightWords[],
				IN  char pchHighlightOpts[],
				IN  char pchReservedOpts1[],
				IN  char pchReservedOpts2[],
				IN  int  pnReserved3[],
				IN  int  pnReserved4[],
				IN  char pchLog[],
				IN  int  nLanguage,
				IN  int  nCharset);


#ifdef __cplusplus
}
#endif

#undef OUT
#undef IN

#endif

