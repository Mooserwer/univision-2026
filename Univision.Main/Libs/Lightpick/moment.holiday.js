//## Moment.JS Holiday Plugin
//
//Usage:
//  Call .holiday() from any moment object. If date is a US Federal Holiday, name of the holiday will be returned.
//  Otherwise, return nothing.
//
//  Example:
//    `moment('12/25/2013').holiday()` will return "Christmas Day"
//
//Holidays:
//  You can configure holiday bellow. The 'M' stands for Month and represents fixed day holidays.
//  The 'W' stands for Week, and represents holidays with date based on week day rules.
//  Example: '10/2/1' Columbus Day (Second monday of october).
//
//License:
//  Copyright (c) 2013 [Jr. Hames](http://jrham.es) under [MIT License](http://opensource.org/licenses/MIT)
(function() {
  var moment;

  moment = typeof require !== "undefined" && require !== null ? require("moment") : this.moment;

  //Holiday definitions
  var _holidays = {
  'M': {//Month, Day
          '01/01': "신정",
          //'01/02': "자율출근",
          '03/01': "삼일절",
          '05/01': "근로자의날",
          '05/05': "어린이날",
          '06/06': "현충일",
          '08/15': "광복절",
          '10/03': "개천절",
          '10/09': "한글날",
          '12/25': "크리스마스",
          '06/03': "지선",
          '09/24': "추석",
          '09/25': "추석",
          '09/26': "추석",
          '02/16': "설날",
          '02/17': "설날",
          '02/18': "설날",
          '03/02': "대체휴일(삼일절)",
          '05/25': "대체휴일(석가탄신일)",
          '08/17': "대체휴일(광복절)",
          '10/05': "대체휴일(개천절)",
          
      },
      'W': {//Month, Week of Month, Day of Week
          //'1/3/1': "Martin Luther King Jr. Day",
          //'2/3/1': "Washington's Birthday",
          //'5/5/1': "Memorial Day",
          //'9/1/1': "Labor Day",
          //'10/2/1': "Columbus Day",
          //'11/4/4': "Thanksgiving Day"
      }
  };

  moment.fn.holiday = function() {
      var diff = 1+ (0 | (this._d.getDate() - 1) / 7),
          memorial = (this._d.getDay() === 1 && (this._d.getDate() + 7) > 30) ? "5" : null;

      return (_holidays['M'][this.format('MM/DD')] || _holidays['W'][this.format('M/'+ (memorial || diff) +'/d')]);
  };

  if ((typeof module !== "undefined" && module !== null ? module.exports : void 0) != null) {
      module.exports = moment;
  }

}(this));
