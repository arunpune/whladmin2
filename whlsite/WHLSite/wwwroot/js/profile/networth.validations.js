$(function () {

  // Disable submission
  $('.whl-action-save-networth').attr('disabled', 'disabled');
  fnToggleRealEstateValueAmt();
  fnToggleSaveNetWorth();

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
    fnToggleSaveNetWorth();
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
    fnToggleSaveNetWorth();
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
    fnToggleSaveNetWorth();
  });

  function fnToggleRealEstateValueAmt() {
    var ore = $('.whl-formfield-ownrealestateind').prop('checked');
    if (ore) {
      $('.whl-formfieldcol-realestatevalueamt').show();
    } else {
      $('.whl-formfieldcol-realestatevalueamt').hide();
      $('.whl-formfield-realestatevalueamt').val('0');
    }
    fnToggleSaveNetWorth();
  }

  function fnToggleSaveNetWorth() {
    var ore = $('.whl-formfield-ownrealestateind').prop('checked');
    var re = $('.whl-formfield-realestatevalueamt').val();
    var inc = $('.whl-formfield-incomevalueamt').val();
    //var ast = $('.whl-formfield-assetvalueamt').val();
    var allowSubmission = !isNaN(parseFloat(re)) && re >= 0.0
      && !isNaN(parseFloat(inc)) && inc >= 0.0;
    if (allowSubmission && ore) {
      allowSubmission = re >= 1.0;
    }
    if (allowSubmission) {
      $('.whl-action-save-networth').removeAttr('disabled');
    }
    else {
      $('.whl-action-save-networth').attr('disabled', 'disabled');
    }
  }

});