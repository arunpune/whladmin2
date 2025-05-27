$(function () {

  // Disable submission
  $('.whl-action-save-addressinfo').attr('disabled', 'disabled');
  fnTogglePhysicalAddresss();
  fnToggleSaveAddressInfo();

  $('.whl-formfield-addressind').on('change', function (e) {
    fnTogglePhysicalAddresss();
    fnToggleSaveAddressInfo();
  });

  $('.whl-formfield-mailingstreetline1').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveAddressInfo();
  });

  $('.whl-formfield-mailingcity').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveAddressInfo();
  });

  $('.whl-formfield-mailingstatecd').on('keypress', function (e) {
    var reg = /^[a-zA-Z]+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-mailingstatecd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length === 2) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveAddressInfo();
  });

  $('.whl-formfield-mailingzipcode').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-mailingzipcode').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length === 5 && !isNaN(parseInt(v))) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveAddressInfo();
  });

  $('.whl-formfield-mailingcounty').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveAddressInfo();
  });

  $('.whl-formfield-mailingaddressind').on('change', function (e) {
    fnToggleMailingAddresss();
    fnToggleSaveAddressInfo();
  });

  $('.whl-formfield-physicalstreetline1').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveAddressInfo();
  });

  $('.whl-formfield-physicalcity').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveAddressInfo();
  });
  $('.whl-formfield-physicalstatecd').on('keypress', function (e) {
    var reg = /^[a-zA-Z]+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-physicalstatecd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length === 2) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveAddressInfo();
  });

  $('.whl-formfield-physicalzipcode').on('keypress', function (e) {
    var reg = /^\d+$/;
    var k = e.key;
    return reg.test(e.key);
  });
  $('.whl-formfield-physicalzipcode').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length === 5 && !isNaN(parseInt(v))) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveAddressInfo();
  });

  $('.whl-formfield-physicalcounty').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSaveAddressInfo();
  });


  function fnTogglePhysicalAddresss() {
    var vi = $('.whl-formfield-addressind').prop('checked');
    if (vi) {
      $('.whl-formfields-physicaladdress').show();
    } else {
      $('.whl-formfields-physicaladdress').hide();
      $('.whl-formfield-physicalstreetline1').val('');
      $('.whl-formfield-physicalstreetline2').val('');
      $('.whl-formfield-physicalstreetline3').val('');
      $('.whl-formfield-physicalcity').val('');
      $('.whl-formfield-physicalstatecd').val('');
      $('.whl-formfield-physicalzipcode').val('');
      $('.whl-formfield-physicalcounty').val('');
      $('.whl-formfield-mailingaddressind').prop('checked', false);
    }
    fnToggleMailingAddresss();
  }

  function fnToggleMailingAddresss() {
    var vi = $('.whl-formfield-mailingaddressind').prop('checked');
    if (vi) {
      $('.whl-formfields-mailingaddress').show();
    } else {
      $('.whl-formfields-mailingaddress').hide();
      $('.whl-formfield-mailingstreetline1').val('');
      $('.whl-formfield-mailingstreetline2').val('');
      $('.whl-formfield-mailingstreetline3').val('');
      $('.whl-formfield-mailingcity').val('');
      $('.whl-formfield-mailingstatecd').val('');
      $('.whl-formfield-mailingzipcode').val('');
      $('.whl-formfield-mailingcounty').val('');
    }
  }

  function fnToggleSaveAddressInfo() {
    var ai = $('.whl-formfield-addressind').prop('checked');
    var psl1 = $('.whl-formfield-physicalstreetline1').val();
    var psl2 = $('.whl-formfield-physicalstreetline2').val();
    var psl3 = $('.whl-formfield-physicalstreetline3').val();
    var psc = $('.whl-formfield-physicalcity').val();
    var pss = $('.whl-formfield-physicalstatecd').val();
    var psz = $('.whl-formfield-physicalzipcode').val();
    var psct = $('.whl-formfield-physicalcounty').val();
    var mai = $('.whl-formfield-mailingaddressind').prop('checked');
    var msl1 = $('.whl-formfield-mailingstreetline1').val();
    var msl2 = $('.whl-formfield-mailingstreetline2').val();
    var msl3 = $('.whl-formfield-mailingstreetline3').val();
    var msc = $('.whl-formfield-mailingcity').val();
    var mss = $('.whl-formfield-mailingstatecd').val();
    var msz = $('.whl-formfield-mailingzipcode').val();
    var msct = $('.whl-formfield-mailingcounty').val();
    var allowSubmission = false;
    if (ai) {
      allowSubmission = (psl1 !== undefined && psl1 !== null && psl1.trim().length > 0)
                          && (psc !== undefined && psc !== null && psc.trim().length > 0)
                          && (pss !== undefined && pss !== null && pss.trim().length === 2)
                          && (psz !== undefined && psz !== null && psz.trim().length === 5 && !isNaN(parseInt(psz)))
                          && (psct !== undefined && psct !== null && psct.trim().length > 0);
      if (allowSubmission && mai) {
        allowSubmission = (msl1 !== undefined && msl1 !== null && msl1.trim().length > 0)
                            && (msc !== undefined && msc !== null && msc.trim().length > 0)
                            && (mss !== undefined && mss !== null && mss.trim().length === 2)
                            && (msz !== undefined && msz !== null && msz.trim().length === 5 && !isNaN(parseInt(msz)))
                            && (msct !== undefined && msct !== null && msct.trim().length > 0);
      }
    } else {
      allowSubmission = true;
    }
    if (allowSubmission) {
      $('.whl-action-save-addressinfo').removeAttr('disabled');
    }
    else {
      $('.whl-action-save-addressinfo').attr('disabled', 'disabled');
    }
  }

});