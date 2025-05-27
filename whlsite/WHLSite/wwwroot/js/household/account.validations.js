$(function () {

  // Disable submission
  $('.whl-action-save-account').attr('disabled', 'disabled');
  fnToggleAccountNumber();
  fnToggleOtherAccount();
  fnToggleSaveAccount();

  $('.whl-formfield-accounttypecd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleAccountNumber();
    fnToggleOtherAccount();
    fnToggleSaveAccount();
  });

  $('.whl-formfield-accounttypeother').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveAccount();
  });

  function fnToggleAccountNumber() {
    var v = $('.whl-formfield-accounttypecd').val();
    if (v !== undefined && v !== null && v.trim() !== '') {
      if (v === 'CASHAPP' || v === 'CRYPTO' || v === 'OTHER') {
        $('.whl-formfield-accountnumber').val('');
        $('.whl-formfieldrow-accountnumber').hide();
        $('.whl-formfieldlabel-accountnumber').removeClass('fw-bolder');
        $('.whl-formfieldrequired-accountnumber').hide();
      } else {
        $('.whl-formfieldrow-accountnumber').show();
        $('.whl-formfieldlabel-accountnumber').addClass('fw-bolder');
        $('.whl-formfieldrequired-accountnumber').show();
      }
    } else {
      $('.whl-formfieldrow-accountnumber').show();
      $('.whl-formfieldlabel-accountnumber').addClass('fw-bolder');
      $('.whl-formfieldrequired-accountnumber').show();
    }
  }

  $('.whl-formfield-accountnumber').on('keypress', function (e) {
    var reg = /^[A-Za-z0-9]+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-accountnumber').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveAccount();
  });

  $('.whl-formfield-accountvalueamt').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-accountvalueamt').on('change', function (e) {
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
    fnToggleSaveAccount();
  });

  $('.whl-formfield-institutionname').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveAccount();
  });

  function fnToggleOtherAccount() {
    $('.whl-formfield-accounttypeother').removeClass('is-valid');
    $('.whl-formfield-accounttypeother').removeClass('is-invalid');
    var v = $('.whl-formfield-accounttypecd').val();
    if (v !== undefined && v !== null && v.trim().length > 0 && v.toUpperCase() === 'OTHER') {
      $('.whl-formfieldcol-accounttypeother').show();
      var ato = $('.whl-formfield-accounttypeother').val();
      if (ato !== undefined && ato !== null && ato.trim().length > 0) {
        $('.whl-formfield-accounttypeother').addClass('is-valid');
      } else {
        $('.whl-formfield-accounttypeother').addClass('is-invalid');
      }
    } else {
      $('.whl-formfieldcol-accounttypeother').hide();
      $('.whl-formfield-accounttypeother').val('');
    }
  }

  function fnToggleSaveAccount() {
    var an = $('.whl-formfield-accountnumber').val();
    var at = $('.whl-formfield-accounttypecd').val();
    var ato = $('.whl-formfield-accounttypeother').val();
    var av = $('.whl-formfield-accountvalueamt').val();
    var nm = $('.whl-formfield-institutionname').val();
    var ph = $('.whl-formfield-primaryholdermemberid').val();
    var allowSubmission = at !== undefined && at !== null && at.trim().length > 0
      && nm !== undefined && nm !== null && nm.trim().length > 0
      && !isNaN(parseFloat(av)) && av >= 0.0
      && !isNaN(parseFloat(ph)) && ph >= 0;
    if (allowSubmission && at.trim().toUpperCase() === 'OTHER') {
      allowSubmission = ato !== undefined && ato !== null && ato.trim().length > 0;
    }
    if (allowSubmission && at.trim().toUpperCase() !== 'CRYPTO'
      && at.trim().toUpperCase() !== 'CASHAPP'
      && at.trim().toUpperCase() !== 'OTHER') {
      allowSubmission = an !== undefined && an !== null && an.trim().length > 0;
    }
    if (allowSubmission) {
      $('.whl-action-save-account').removeAttr('disabled');
    }
    else {
      $('.whl-action-save-account').attr('disabled', 'disabled');
    }
  }

});