$(function () {

  // Disable submission
  $('.whl-action-save-profile').attr('disabled', 'disabled');
  fnToggleSaveProfile();

  $('.whl-formfield-firstname').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (fnIsValidNameWithSpecialCharacters(v)) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveProfile();
  });

  $('.whl-formfield-middlename').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      if (fnIsValidNameWithSpecialCharacters(v)) {
        $(this).addClass('is-valid');
      } else {
        $(this).addClass('is-invalid');
      }
    }
    fnToggleSaveProfile();
  });

  $('.whl-formfield-lastname').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (fnIsValidNameWithSpecialCharacters(v)) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveProfile();
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
    fnToggleSaveProfile();
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
    fnToggleSaveProfile();
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
    fnToggleSaveProfile();
  });

  $('.whl-formfield-idtypecd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveProfile();
  });

  $('.whl-formfield-idtypevalue').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveProfile();
  });

  $('.whl-formfield-idissuedate').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveProfile();
  });

  $('.whl-formfield-gendercd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveProfile();
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
    fnToggleSaveProfile();
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
    fnToggleSaveProfile();
  });

  $('.whl-formfield-householdsize').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-householdsize').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = parseInt($(this).val());
    if (!isNaN(v) && v >= 1 && v <= 12) {
      $(this).addClass('is-valid');
    } else {
      $(this).val('1');
    }
    fnToggleSaveProfile();
  });

  function fnToggleSaveProfile() {
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
    var hhsz = parseInt($('.whl-formfield-householdsize').val());
    var allowSubmission = fnIsValidNameWithSpecialCharacters(fn)
                            && fnIsValidNameWithSpecialCharacters(ln)
                            && ss !== undefined && ss !== null && ss.trim().length === 4
                            && db !== undefined && db !== null && db.trim().length > 0
                            && idtc !== undefined && idtc !== null && idtc.trim().length > 0
                            && idtv !== undefined && idtv !== null && idtv.trim().length > 0
                            && idid !== undefined && idid !== null && idid.trim().length > 0
                            && gc !== undefined && gc !== null && gc.trim().length > 0
                            && rc !== undefined && rc !== null && rc.trim().length > 0
                            && ec !== undefined && ec !== null && ec.trim().length > 0
                            && !isNaN(hhsz) && hhsz >= 1 && hhsz <= 12;
    if (allowSubmission && mn !== undefined && mn !== null && mn.trim().length > 0) {
      allowSubmission = fnIsValidName(mn);
    }
    if (allowSubmission && sf !== undefined && sf !== null && sf.trim().length > 0) {
      allowSubmission = fnIsValidName(sf);
    }
    if (allowSubmission) {
      $('.whl-action-save-profile').removeAttr('disabled');
    }
    else {
      $('.whl-action-save-profile').attr('disabled', 'disabled');
    }
  }

  //$('.whl-form-save-profile').on('submit', function (e) {
  //  e.preventDefault();
  //  e.stopPropagation();
  //  console.log('formsubmission');
  //});

});