$(function () {

  // Disable submission
  $('.whl-action-save-member').attr('disabled', 'disabled');
  fnToggleOtherRelation();
  fnToggleRealEstateValueAmt();
  fnToggleSaveMember();

  $('.whl-formfield-relationtypecd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleOtherRelation();
    fnToggleSaveMember();
  });

  $('.whl-formfield-relationtypeother').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-firstname').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (fnIsValidName(v)) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-middlename').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      if (fnIsValidName(v)) {
        $(this).addClass('is-valid');
      } else {
        $(this).addClass('is-invalid');
      }
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-lastname').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (fnIsValidName(v)) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-suffix').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      if (fnIsValidSuffix(v)) {
        $(this).addClass('is-valid');
      } else {
        $(this).addClass('is-invalid');
      }
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-last4ssn').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-last4ssn').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length === 4) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-dateofbirth').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveMember();
  });

  // $('.whl-formfield-idtypecd').on('change', function (e) {
  //   $(this).removeClass('is-valid');
  //   $(this).removeClass('is-invalid');
  //   var v = $(this).val();
  //   if (v !== undefined && v !== null && v.trim().length > 0) {
  //     $(this).addClass('is-valid');
  //   } else {
  //     $(this).addClass('is-invalid');
  //   }
  //   fnToggleSaveMember();
  // });

  // $('.whl-formfield-idtypevalue').on('change', function (e) {
  //   $(this).removeClass('is-valid');
  //   $(this).removeClass('is-invalid');
  //   var v = $(this).val();
  //   if (v !== undefined && v !== null && v.trim().length > 0) {
  //     $(this).addClass('is-valid');
  //   } else {
  //     $(this).addClass('is-invalid');
  //   }
  //   fnToggleSaveMember();
  // });

  // $('.whl-formfield-idissuedate').on('change', function (e) {
  //   $(this).removeClass('is-valid');
  //   $(this).removeClass('is-invalid');
  //   var v = $(this).val();
  //   if (v !== undefined && v !== null && v.trim().length > 0) {
  //     $(this).addClass('is-valid');
  //   } else {
  //     $(this).addClass('is-invalid');
  //   }
  //   fnToggleSaveMember();
  // });

  $('.whl-formfield-gendercd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-racecd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-ethnicitycd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-phonenumber').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-phonenumber').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      if (fnIsValidPhoneNumber(v)) {
        $(this).addClass('is-valid');
      } else {
        $(this).addClass('is-invalid');
      }
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-phonenumberextn').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-phonenumberextn').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var n = $('.whl-formfield-phonenumber').val();
    if (n !== undefined && n !== null && n.trim().length > 0 && fnIsValidPhoneNumber(n)) {
      var v = $(this).val();
      if (v === '') {
        // do nothing
      } else {
        if (v === '' || !isNaN(parseInt(v))) {
          $(this).addClass('is-valid');
        } else {
          $(this).addClass('is-invalid');
        }
      }
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-phonenumbertypecd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var n = $('.whl-formfield-phonenumber').val();
    if (n !== undefined && n !== null && n.trim().length > 0 && fnIsValidPhoneNumber(n)) {
      var v = $(this).val();
      if (v !== undefined && v !== null && v.trim().length > 0) {
        $(this).addClass('is-valid');
      } else {
        $(this).addClass('is-invalid');
      }
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-altphonenumber').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-altphonenumber').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      if (fnIsValidPhoneNumber(v)) {
        $(this).addClass('is-valid');
      } else {
        $(this).addClass('is-invalid');
      }
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-altphonenumberextn').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-altphonenumberextn').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var n = $('.whl-formfield-phonenumber').val();
    if (n !== undefined && n !== null && n.trim().length > 0 && fnIsValidPhoneNumber(n)) {
      var v = $(this).val();
      if (v === '') {
        // do nothing
      } else {
        if (v === '' || !isNaN(parseInt(v))) {
          $(this).addClass('is-valid');
        } else {
          $(this).addClass('is-invalid');
        }
      }
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-altphonenumbertypecd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var n = $('.whl-formfield-phonenumber').val();
    if (n !== undefined && n !== null && n.trim().length > 0 && fnIsValidPhoneNumber(n)) {
      var v = $(this).val();
      if (v !== undefined && v !== null && v.trim().length >= 0) {
        $(this).addClass('is-valid');
      } else {
        $(this).addClass('is-invalid');
      }
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-emailaddress').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    var aem = $('.whl-formfield-altemailaddress').val();
    if (fnIsValidEmailAddress(v) && aem.toLowerCase() !== v.toLowerCase()) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-altemailaddress').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    var em = $('.whl-formfield-emailaddress').val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      if (fnIsValidEmailAddress(v) && em.toLowerCase() !== v.toLowerCase()) {
        $(this).addClass('is-valid');
      } else {
        $(this).addClass('is-invalid');
      }
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-incomevalueamt').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-incomevalueamt').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v === '') {
      $(this).val('0');
    } else {
      if (v === '' || !isNaN(parseFloat(v))) {
        $(this).addClass('is-valid');
      } else {
        $(this).addClass('is-invalid');
      }
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-assetvalueamt').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-assetvalueamt').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v === '') {
      $(this).val(0);
    } else {
      if (v === '' || !isNaN(parseFloat(v))) {
        $(this).addClass('is-valid');
      } else {
        $(this).addClass('is-invalid');
      }
    }
    fnToggleSaveMember();
  });

  $('.whl-formfield-ownrealestateind').on('change', function (e) {
    fnToggleRealEstateValueAmt();
  });

  $('.whl-formfield-realestatevalueamt').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-realestatevalueamt').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v === '') {
      $(this).val('0');
    } else {
      if (v === '' || !isNaN(parseFloat(v))) {
        $(this).addClass('is-valid');
      } else {
        $(this).addClass('is-invalid');
      }
    }
    fnToggleSaveMember();
  });

  function fnToggleOtherRelation() {
    $('.whl-formfield-relationtypeother').removeClass('is-valid');
    $('.whl-formfield-relationtypeother').removeClass('is-invalid');
    var v = $('.whl-formfield-relationtypecd').val();
    if (v !== undefined && v !== null && v.trim().length > 0 && v.toUpperCase() === 'OTHER') {
      $('.whl-formfieldcol-relationtypeother').show();
      var rto = $('.whl-formfield-relationtypeother').val();
      if (rto !== undefined && rto !== null && rto.trim().length > 0) {
        $('.whl-formfield-relationtypeother').addClass('is-valid');
      } else {
        $('.whl-formfield-relationtypeother').addClass('is-invalid');
      }
    } else {
      $('.whl-formfieldcol-relationtypeother').hide();
      $('.whl-formfield-relationtypeother').val('');
    }
  }

  function fnToggleRealEstateValueAmt() {
    var ore = $('.whl-formfield-ownrealestateind').prop('checked');
    if (ore) {
      $('.whl-formfieldcol-realestatevalueamt').show();
    } else {
      $('.whl-formfieldcol-realestatevalueamt').hide();
      $('.whl-formfield-realestatevalueamt').val('0');
    }
  }

  function fnToggleSaveMember() {
    var rt = $('.whl-formfield-relationtypecd').val();
    var rto = $('.whl-formfield-relationtypeother').val();
    var ti = $('.whl-formfield-title').val();
    var fn = $('.whl-formfield-firstname').val();
    var mn = $('.whl-formfield-middlename').val();
    var ln = $('.whl-formfield-lastname').val();
    var sf = $('.whl-formfield-suffix').val();
    var ss = $('.whl-formfield-last4ssn').val();
    var db = $('.whl-formfield-dateofbirth').val();
    var idtc = $('.whl-formfield-idtypecd').val();
    var idtv = $('.whl-formfield-idtypevalue').val();
    var idid = $('.whl-formfield-idissuedate').val();
    var gc = $('.whl-formfield-gendercd').val();
    var rc = $('.whl-formfield-racecd').val();
    var ec = $('.whl-formfield-ethnicitycd').val();
    var pr = $('.whl-formfield-pronouns').val();
    var st = $('.whl-formfield-studentind').val();
    var ds = $('.whl-formfield-disabilityind').val();
    var vt = $('.whl-formfield-veteranind').val();
    var em = $('.whl-formfield-emailaddress').val();
    var aem = $('.whl-formfield-altemailaddress').val();
    var ore = $('.whl-formfield-ownrealestateind').prop('checked');
    var re = $('.whl-formfield-realestatevalueamt').val();
    var inc = $('.whl-formfield-incomevalueamt').val();
    //var ast = $('.whl-formfield-assetvalueamt').val();
    var allowSubmission = rt !== undefined && rt !== null && rt.trim().length > 0
      && fnIsValidName(fn)
      && fnIsValidName(ln)
      && ss !== undefined && ss !== null && ss.trim().length === 4
      && db !== undefined && db !== null && db.trim().length > 0
      // && idtc !== undefined && idtc !== null && idtc.trim().length > 0
      // && idtv !== undefined && idtv !== null && idtv.trim().length > 0
      // && idid !== undefined && idid !== null && idid.trim().length > 0
      && gc !== undefined && gc !== null && gc.trim().length > 0
      && rc !== undefined && rc !== null && rc.trim().length > 0
      && ec !== undefined && ec !== null && ec.trim().length > 0
      && !isNaN(parseFloat(re)) && re >= 0.0
      && !isNaN(parseFloat(inc)) && inc >= 0.0;
    if (allowSubmission && mn !== undefined && mn !== null && mn.trim().length > 0) {
      allowSubmission = fnIsValidName(mn);
    }
    if (allowSubmission && ore) {
      allowSubmission = re >= 1.0;
    }
    if (allowSubmission && rt.trim().toUpperCase() === 'OTHER') {
      allowSubmission = rto !== undefined && rto !== null && rto.trim().length > 0;
    }
    if (allowSubmission) {
      if (em !== undefined && em !== null && em.trim().length > 0) {
        if (fnIsValidEmailAddress(em)) {
          if (aem !== undefined && aem !== null && aem.trim().length > 0) {
            if (fnIsValidEmailAddress(aem)) {
              allowSubmission = em.toLowerCase() !== aem.toLowerCase();
            } else {
              allowSubmission = false;
            }
          }
        } else {
          allowSubmission = false;
        }
      }
    }
    if (allowSubmission) {
      $('.whl-action-save-member').removeAttr('disabled');
    }
    else {
      $('.whl-action-save-member').attr('disabled', 'disabled');
    }
  }

});