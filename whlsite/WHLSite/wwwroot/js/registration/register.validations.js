let usernameAvailable = false;
$(function () {

  // Disable submission
  $('.whl-action-register').attr('disabled', 'disabled');
  fnToggleCheckAvailability();
  fnToggleLeadTypeOther();
  fnToggleRegister();

  $('.whl-formfield-username').on('keypress', function (e) {
    var v = $(this).val();
    var k = e.key;
    if (v.length > 0) {
      return /^[A-Za-z0-9]$/.test(e.key);
    } else {
      return /^[A-Za-z]$/.test(e.key);
    }
  });
  $('.whl-formfield-username').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (fnIsValidUsername(v)) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleRegister();
    fnToggleCheckAvailability();
  });

  function fnToggleCheckAvailability() {
    usernameAvailable = false;
    $('.whl-action-checkavailability').show();
    var v = $('.whl-formfield-username').val();
    if (v !== undefined && v !== null && v.trim().length >= 8 && fnIsValidUsername(v)) {
      $('.whl-action-checkavailability').removeAttr('disabled');
    } else {
      $('.whl-action-checkavailability').attr('disabled', 'disabled');
    }
    $('.whl-formfieldlabel-usernameavailable').hide();
    $('.whl-formfieldlabel-usernameavailable').html('');
    $('.whl-formfieldlabel-usernameavailable').removeClass('text-success');
    $('.whl-formfieldlabel-usernameavailable').removeClass('text-danger');
  }

  $('.whl-formfield-emailaddress').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (fnIsValidEmailAddress(v)) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleRegister();
  });

  $('.whl-formfield-password').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (fnIsValidPassword(v)) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleRegister();
  });

  $('.whl-formfield-confirmpassword').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v1 = $('.whl-formfield-password').val();
    var v = $(this).val();
    if (fnIsValidPassword(v) && v1 === v) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleRegister();
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
    if (fnIsValidPhoneNumber(v)) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleRegister();
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
    fnToggleRegister();
  });

  $('.whl-formfield-phonenumberextn').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });

  $('.whl-formfield-leadtypecd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
      fnToggleLeadTypeOther();
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleRegister();
  });

  $('.whl-formfield-leadtypeother').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleRegister();
  });

  $('.whl-formfield-emailconsent').on('change', function (e) {
    fnToggleRegister();
  });

  $('.whl-formfield-acceptterms').on('change', function (e) {
    fnToggleRegister();
  });

  function fnToggleLeadTypeOther() {
    $('.whl-formfield-leadtypeother').removeClass('is-valid');
    $('.whl-formfield-leadtypeother').removeClass('is-invalid');
    var lt = $('.whl-formfield-leadtypecd').val();
    var lto = $('.whl-formfield-leadtypeother').val();
    if (lt === 'WEBSITE' || lt === 'NEWSPAPERART' || lt === 'OTHER') {
      $('.whl-formfieldcol-leadtypeother').show();
      if (lto !== undefined && lto !== null && lto.trim().length > 0) {
        $('.whl-formfield-leadtypeother').addClass('is-valid');
      } else {
        $('.whl-formfield-leadtypeother').addClass('is-invalid');
      }
    } else {
      $('.whl-formfieldcol-leadtypeother').hide();
      $('.whl-formfield-leadtypeother').val('');
    }
  }

  function fnToggleRegister() {
    var un = $('.whl-formfield-username').val();
    var em = $('.whl-formfield-emailaddress').val();
    var pw = $('.whl-formfield-password').val();
    var cpw = $('.whl-formfield-confirmpassword').val();
    var pn = $('.whl-formfield-phonenumber').val();
    var pne = $('.whl-formfield-phonenumberextn').val();
    var pt = $('.whl-formfield-phonenumbertypecd').val();
    var lt = $('.whl-formfield-leadtypecd').val();
    var lto = $('.whl-formfield-leadtypeother').val();
    var eml = $('.whl-formfield-emailconsent').prop('checked');
    var tnc = $('.whl-formfield-acceptterms').prop('checked');
    var allowSubmission = fnIsValidUsername(un) && usernameAvailable && fnIsValidEmailAddress(em)
                            && fnIsValidPassword(pw) && fnIsValidPassword(cpw)
                            && pw === cpw
                            && fnIsValidPhoneNumber(pn)
                            && (pne === '' || !isNaN(parseInt(pne)))
                            && (pt !== undefined && pt !== null && pt.trim().length > 0)
                            && (lt !== undefined && lt !== null && lt.trim().length > 0)
                            && eml && tnc;
    if (lt === 'WEBSITE' || lt === 'NEWSPAPERART' || lt === 'OTHER') {
      allowSubmission = allowSubmission && (lto !== undefined && lto !== null && lto.trim().length > 0);
    }
    if (allowSubmission) {
      $('.whl-action-register').removeAttr('disabled');
    }
    else {
      $('.whl-action-register').attr('disabled', 'disabled');
    }
  }

  $('.whl-action-checkavailability').on('click', function (e) {
    usernameAvailable = false;
    $('.whl-formfieldlabel-usernameavailable').hide();
    $('.whl-formfieldlabel-usernameavailable').removeClass('text-success');
    $('.whl-formfieldlabel-usernameavailable').removeClass('text-danger');
    $('.whl-formfieldlabel-usernameavailable').html('');
    e.preventDefault();
    e.stopPropagation();
    var u = $('.whl-formfield-username').val();
    if (u === undefined || u === null) u = '';
    u = u.trim();
    var e = $('.whl-formfield-emailaddress').val();
    if (e === undefined || e === null) e = '';
    e = e.trim();
    $('.whl-formfield-username').removeClass('is-valid');
    $('.whl-formfield-username').removeClass('is-invalid');
    $.get(checkAvailabilityUrl + '?u=' + u + '&e=' + e)
      .done(function (response) {
        $('.whl-formfieldlabel-usernameavailable').show();
        $('.whl-formfieldlabel-usernameavailable').html('Available');
        $('.whl-formfieldlabel-usernameavailable').addClass('text-success');
        $('.whl-action-checkavailability').hide();
        $('.whl-formfield-username').addClass('is-valid');
        usernameAvailable = true;
        fnToggleRegister();
      }).fail(function (err) {
        $('.whl-formfieldlabel-usernameavailable').show();
        $('.whl-formfieldlabel-usernameavailable').html('Not Available');
        $('.whl-formfieldlabel-usernameavailable').addClass('text-danger');
        $('.whl-action-checkavailability').show();
        $('.whl-formfield-username').addClass('is-invalid');
        fnToggleRegister();
      });
  });

});