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
#ifndef __KSEARCH2_H_
#define __KSEARCH2_H_

#define OUT
#define IN

#ifdef __cplusplus
extern	"C" {
#endif

__declspec(dllimport)int KSEARCH_Search (
                OUT char msg[],
                IN  H_KSEARCH hc,
                IN  char pchServiceAddr[],
                IN  char pchScn[],
                IN  char pchWhereClause[],
                IN  char pchSortingClause[],
                IN  char pchSearchWords[],
                IN  char pchLog[],
                IN  int  nStartOffset,
                IN  int  nRecordCount,
                IN  int  nLanguage,
                IN  int  nCharset);

__declspec(dllimport)int KSEARCH_GetResult_GroupBy (
                OUT char   msg[],
                IN  H_KSEARCH   hc,
                OUT int*   pnGroupCount,
                OUT int*   pnGroupKeyCount,
                OUT char** pppchGroupKeyVal[],
                OUT int    pnGroupSize[],
				IN	int    nMaxGroupCount);

__declspec(dllimport)int KSEARCH_GetResult_ROWID2 (
                OUT char  msg[],
                IN  H_KSEARCH  hc,
                OUT int*  pnRowidCount,
                OUT int   pnTableNo[],
				OUT char* ppchLinkName[],
				OUT char* ppchVolumeName[],
				OUT char* ppchTableName[],
                OUT int   pnRowid[],
                OUT unsigned int pnScore[],
				IN  int   nMaxRowidCount);

__declspec(dllimport)int KSEARCH_PutCache (
				OUT char   msg[],
				IN  H_KSEARCH   hc,
				IN  char   pchServiceAddr[],
				IN  int    nKeySize,
				IN  char   pchKey[],
				IN  int    nDataSize,
				IN  char   pchData[],
				IN  char   pchPriorityKey[],
				IN  int    nDomainNo);

__declspec(dllimport)int KSEARCH_GetCache (
				OUT char   msg[],
				IN  H_KSEARCH   hc,
				IN  char   pchServiceAddr[],
				OUT int*   pnHitFlag,
				OUT int*   pnOutDataSize,
				OUT char** ppchOutData,
				IN  int    nKeySize,
				IN  char   pchKey[],
				IN  int    nDomainNo);

#ifdef __cplusplus
}
#endif

#undef OUT
#undef IN

#endif

