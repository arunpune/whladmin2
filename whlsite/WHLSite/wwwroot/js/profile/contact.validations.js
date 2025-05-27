$(function () {

  // Disable submission
  $('.whl-action-save-contactinfo').attr('disabled', 'disabled');
  fnToggleSaveContactInfo();

  $('.whl-formfield-phonenumber').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-phonenumber').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (fnIsValidPhoneNumber(v)) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveContactInfo();
  });

  $('.whl-formfield-phonenumberextn').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-phonenumberextn').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
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
    fnToggleSaveContactInfo();
  });

  $('.whl-formfield-phonenumbertypecd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveContactInfo();
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
    fnToggleSaveContactInfo();
  });

  $('.whl-formfield-altphonenumberextn').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-altphonenumberextn').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
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
    fnToggleSaveContactInfo();
  });

  $('.whl-formfield-altphonenumbertypecd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length >= 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveContactInfo();
  });

  $('.whl-formfield-emailaddress').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    var u = $('.whl-formfield-altemailaddress').val();
    if (fnIsValidEmailAddress(v) && u.toLowerCase() !== v.toLowerCase()) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveContactInfo();
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
    fnToggleSaveContactInfo();
  });

  function fnToggleSaveContactInfo() {
    var pn = $('.whl-formfield-phonenumber').val();
    var pne = $('.whl-formfield-phonenumberextn').val();
    var pt = $('.whl-formfield-phonenumbertypecd').val();
    var apn = $('.whl-formfield-altphonenumber').val();
    var apne = $('.whl-formfield-altphonenumberextn').val();
    var apt = $('.whl-formfield-altphonenumbertypecd').val();
    var em = $('.whl-formfield-emailaddress').val();
    var ae = $('.whl-formfield-altemailaddress').val();
    var u = $('.whl-formfield-username').val();
    var allowSubmission = fnIsValidPhoneNumber(pn)
      && (pne === '' || !isNaN(parseInt(pne)))
      && (pt !== undefined && pt !== null && pt.trim().length > 0)
      && (apn === '' || (fnIsValidPhoneNumber(apn)
        && (apne === '' || !isNaN(parseInt(apne)))
        && (apt !== undefined && apt !== null && apt.trim().length > 0)))
      && fnIsValidEmailAddress(em)
      && (ae === '' || (fnIsValidEmailAddress(ae) && em.toLowerCase() !== ae.toLowerCase()));
    if (allowSubmission) {
      $('.whl-action-save-contactinfo').removeAttr('disabled');
    }
    else {
      $('.whl-action-save-contactinfo').attr('disabled', 'disabled');
    }
  }

});