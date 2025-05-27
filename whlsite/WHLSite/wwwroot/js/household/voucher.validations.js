$(function () {

  // Disable submission
  $('.whl-action-save-voucherinfo').attr('disabled', 'disabled');
  fnToggleVouchers();
  fnToggleSaveVoucherInfo();

  $('.whl-formfield-voucherind').on('change', function (e) {
    fnToggleVouchers();
    fnToggleSaveVoucherInfo();
  });

  $('.whl-formfield-vouchercd').on('change', function (e) {
    var v = '';
    $('.whl-formfield-vouchercd').each((i, o) => {
      if ($(o).prop('checked')) {
        if ($(o).val() === 'OTHER') oc = true;
        v += (v.length > 0 ? ',' : '') + $(o).val();
      }
    });
    $('.whl-formfield-vouchercds').val(v);
    fnToggleVoucherOther();
    fnToggleSaveVoucherInfo();
  });

  $('.whl-formfield-voucherother').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveVoucherInfo();
  });

  $('.whl-formfield-voucheradminname').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveVoucherInfo();
  });

  function fnToggleVouchers() {
    var vi = $('.whl-formfield-voucherind').prop('checked');
    if (vi) {
      $('.whl-formfieldrow-vouchercds').show();
      $('.whl-formfieldrow-voucheradminname').show();
    } else {
      $('.whl-formfield-vouchercd').each((i, o) => {
        $(o).prop('checked', false);
      });
      $('.whl-formfieldrow-vouchercds').hide();
      $('.whl-formfield-vouchercds').val('');
      $('.whl-formfield-voucherother').val('');
      $('.whl-formfieldrow-voucheradminname').hide();
      $('.whl-formfield-voucheradminname').val('');
    }
    fnToggleVoucherOther();
  }

  function fnToggleVoucherOther() {
    $('.whl-formfield-voucherother').removeClass('is-valid');
    $('.whl-formfield-voucherother').removeClass('is-invalid');
    var v = $('.whl-formfield-vouchercds').val();
    if (v.indexOf('OTHER') > -1) {
      $('.whl-formfieldrow-voucherother').show();
      var vto = $('.whl-formfield-voucherother').val();
      if (vto !== undefined && vto !== null && vto.trim().length > 0) {
        $('.whl-formfield-voucherother').addClass('is-valid');
      } else {
        $('.whl-formfield-voucherother').addClass('is-invalid');
      }
    } else {
      $('.whl-formfieldrow-voucherother').hide();
      $('.whl-formfield-voucherother').val('');
    }
  }

  function fnToggleSaveVoucherInfo() {
    var vi = $('.whl-formfield-voucherind').prop('checked');
    var vt = $('.whl-formfield-vouchercds').val();
    var vto = $('.whl-formfield-voucherother').val();
    var va = $('.whl-formfield-voucheradminname').val();
    var allowSubmission = false;
    if (vi) {
      allowSubmission = vt !== undefined && vt !== null && vt.trim().length > 0
        && va !== undefined && va !== null && va.trim().length > 0;
      if (allowSubmission && vt.indexOf('OTHER') > -1) {
        allowSubmission = vto !== undefined && vto !== null && vto.trim().length > 0;
      }
    } else {
      allowSubmission = true;
    }
    if (allowSubmission) {
      $('.whl-action-save-voucherinfo').removeAttr('disabled');
    }
    else {
      $('.whl-action-save-voucherinfo').attr('disabled', 'disabled');
    }
  }

});